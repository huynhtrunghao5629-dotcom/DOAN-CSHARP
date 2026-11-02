using HotelManagement.Desktop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HotelManager.UI
{
    public partial class FrmManageInvoices : Form
    {
        private List<BookingViewModel> _occupiedBookings = new List<BookingViewModel>();
        private BookingViewModel? _selectedBooking;
        private List<ChiTietDichVuViewModel> _serviceDetails = new List<ChiTietDichVuViewModel>();

        public FrmManageInvoices()
        {
            InitializeComponent();
        }

        private async void FrmManageInvoices_Load(object sender, EventArgs e)
        {
            SetupDataGridView();
            dtpCheckOut.Value = DateTime.Now;
            await LoadOccupiedBookingsAsync();
        }

        private void SetupDataGridView()
        {
            dgvServices.AutoGenerateColumns = true;
            dgvServices.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvServices.ReadOnly = true;
        }

        // 1. TẢI CÁC ĐƠN ĐẶT PHÒNG ĐANG Ở (DaNhanPhong)
        private async Task LoadOccupiedBookingsAsync()
        {
            try
            {
                using (HttpClient client = BaseApiClient.GetClient())
                {
                    HttpResponseMessage response = await client.GetAsync("api/DonDatPhong");
                    if (response.IsSuccessStatusCode)
                    {
                        var allBookings = await response.Content.ReadFromJsonAsync<List<BookingViewModel>>();
                        _occupiedBookings = allBookings?.Where(b => b.TrangThai == "DaNhanPhong").ToList() ?? new List<BookingViewModel>();

                        cboOccupiedBookings.DataSource = _occupiedBookings;
                        cboOccupiedBookings.DisplayMember = "SoPhong";
                        cboOccupiedBookings.ValueMember = "Id";

                        if (_occupiedBookings.Count == 0)
                        {
                            ClearInvoiceDetails();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách đơn lưu trú: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 2. TẢI CHI TIẾT DỊCH VỤ PHÒNG ĐÃ DÙNG
        private async Task LoadServicesForRoomAsync(string phongId)
        {
            if (string.IsNullOrEmpty(phongId))
            {
                _serviceDetails = new List<ChiTietDichVuViewModel>();
                dgvServices.DataSource = null;
                return;
            }

            try
            {
                using (HttpClient client = BaseApiClient.GetClient())
                {
                    HttpResponseMessage response = await client.GetAsync($"api/ChiTietDichVu/phong/{phongId}");
                    if (response.IsSuccessStatusCode)
                    {
                        _serviceDetails = await response.Content.ReadFromJsonAsync<List<ChiTietDichVuViewModel>>() ?? new List<ChiTietDichVuViewModel>();
                        dgvServices.DataSource = _serviceDetails;
                    }
                }
            }
            catch
            {
                _serviceDetails = new List<ChiTietDichVuViewModel>();
                dgvServices.DataSource = null;
            }
        }

        // 3. TÍNH TOÁN VÀ HIỂN THỊ TỔNG TIỀN
        private void CalculateInvoiceTotals()
        {
            if (_selectedBooking == null) return;

            DateTime checkIn = _selectedBooking.NgayNhanPhong;
            DateTime checkOut = dtpCheckOut.Value;

            int days = Math.Max(1, (checkOut.Date - checkIn.Date).Days);
            decimal roomRate = _selectedBooking.TongTien > 0 ? _selectedBooking.TongTien : 500000; // Mặc định hoặc lấy theo đơn
            decimal roomTotal = days * roomRate;
            decimal serviceTotal = _serviceDetails.Sum(s => s.ThanhTien);
            decimal grandTotal = roomTotal + serviceTotal;

            txtDays.Text = $"{days} ngày";
            txtRoomRate.Text = $"{roomRate:N0} VNĐ";
            txtRoomTotal.Text = $"{roomTotal:N0} VNĐ";
            txtServiceTotal.Text = $"{serviceTotal:N0} VNĐ";
            lblGrandTotalAmount.Text = $"{grandTotal:N0} VNĐ";
        }

        // 4. THỰC HIỆN THANH TOÁN & CHECK-OUT QUA API
        private async void btnPay_Click(object sender, EventArgs e)
        {
            if (_selectedBooking == null)
            {
                MessageBox.Show("Vui lòng chọn đơn đặt phòng cần thanh toán!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show($"Xác nhận thanh toán cho khách hàng '{_selectedBooking.TenKhachHang}' (Phòng {_selectedBooking.SoPhong}) với tổng tiền {lblGrandTotalAmount.Text}?",
                                         "Xác nhận thanh toán", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                using (HttpClient client = BaseApiClient.GetClient())
                {
                    // a. Cập nhật trạng thái Đơn đặt phòng -> 'DaThanhToan'
                    var payloadStatus = new { TrangThai = "DaThanhToan" };
                    HttpResponseMessage respBooking = await client.PutAsJsonAsync($"api/DonDatPhong/{_selectedBooking.Id}/trang-thai", payloadStatus);

                    // b. Cập nhật trạng thái Phòng -> 'Trong'
                    if (!string.IsNullOrEmpty(_selectedBooking.SoPhong))
                    {
                        var payloadRoom = new { TrangThai = "Trong" };
                        await client.PutAsJsonAsync($"api/Phong/{_selectedBooking.SoPhong}/trang-thai", payloadRoom);
                    }

                    if (respBooking.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Thanh toán và làm thủ tục Trả phòng thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        await LoadOccupiedBookingsAsync();
                    }
                    else
                    {
                        MessageBox.Show($"Thanh toán thất bại. Mã lỗi API: {respBooking.StatusCode}", "Lỗi API", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kết nối khi thanh toán: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 5. IN / XEM TRƯỚC HÓA ĐƠN
        private void btnPrintInvoice_Click(object sender, EventArgs e)
        {
            if (_selectedBooking == null)
            {
                MessageBox.Show("Vui lòng chọn đơn đặt phòng để xem hóa đơn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("==========================================");
            sb.AppendLine("         HÓA ĐƠN THANH TOÁN KHÁCH SẠN");
            sb.AppendLine("==========================================");
            sb.AppendLine($"Khách hàng : {_selectedBooking.TenKhachHang}");
            sb.AppendLine($"Số phòng   : {_selectedBooking.SoPhong}");
            sb.AppendLine($"Ngày nhận  : {_selectedBooking.NgayNhanPhong:dd/MM/yyyy HH:mm}");
            sb.AppendLine($"Ngày trả   : {dtpCheckOut.Value:dd/MM/yyyy HH:mm}");
            sb.AppendLine($"Số ngày ở  : {txtDays.Text}");
            sb.AppendLine($"Tiền phòng : {txtRoomTotal.Text}");
            sb.AppendLine("------------------------------------------");
            sb.AppendLine("DỊCH VỤ SỬ DỤNG:");
            foreach (var s in _serviceDetails)
            {
                sb.AppendLine($" - {s.TenDichVu} (x{s.SoLuong}): {s.ThanhTien:N0} VNĐ");
            }
            sb.AppendLine($"Tiền dịch vụ: {txtServiceTotal.Text}");
            sb.AppendLine("------------------------------------------");
            sb.AppendLine($"TỔNG CỘNG  : {lblGrandTotalAmount.Text}");
            sb.AppendLine("==========================================");
            sb.AppendLine("   Cảm ơn Quý khách & Hẹn gặp lại!");

            MessageBox.Show(sb.ToString(), "Chi tiết Hóa đơn thanh toán", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // HELPER & SỰ KIỆN GIAO DIỆN
        private async void cboOccupiedBookings_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboOccupiedBookings.SelectedItem is BookingViewModel booking)
            {
                _selectedBooking = booking;
                txtCustomerName.Text = booking.TenKhachHang;
                txtRoomNumber.Text = booking.SoPhong;
                txtCheckIn.Text = booking.NgayNhanPhong.ToString("dd/MM/yyyy HH:mm");

                await LoadServicesForRoomAsync(booking.SoPhong ?? "");
                CalculateInvoiceTotals();
            }
        }

        private void dtpCheckOut_ValueChanged(object sender, EventArgs e)
        {
            CalculateInvoiceTotals();
        }

        private void ClearInvoiceDetails()
        {
            _selectedBooking = null;
            txtCustomerName.Clear();
            txtRoomNumber.Clear();
            txtCheckIn.Clear();
            txtDays.Clear();
            txtRoomRate.Clear();
            txtRoomTotal.Clear();
            txtServiceTotal.Clear();
            lblGrandTotalAmount.Text = "0 VNĐ";
            dgvServices.DataSource = null;
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await LoadOccupiedBookingsAsync();
        }
    }
}