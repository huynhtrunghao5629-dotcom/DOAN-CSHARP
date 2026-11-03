using HotelManager.Services;
using HotelManagement.Desktop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HotelManager.UI
{
    public partial class FrmManageInvoices : Form
    {
        private List<BookingViewModel> _occupiedBookings = new();
        private BookingViewModel? _selectedBooking;
        private List<ChiTietDichVuViewModel> _serviceDetails = new();
        private bool _servicesLoaded;
        private bool _loadingBookings;

        public FrmManageInvoices()
        {
            InitializeComponent();
        }

        private async void FrmManageInvoices_Load(object sender, EventArgs e)
        {
            dgvServices.AutoGenerateColumns = true;
            dgvServices.DataBindingComplete += (sender, args) => TieuDeTiengViet.GanCho(dgvServices);
            dgvServices.BackgroundColor = System.Drawing.Color.White;
            dgvServices.ReadOnly = true;
            dgvServices.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            // Không thể ghi thanh toán + check-out an toàn bằng hai request độc lập.
            btnPay.Enabled = false;
            btnPay.Text = "Thanh toán (chờ API)";
            btnPrintInvoice.Text = "Xem bảng tính tạm";
            dtpCheckOut.Value = DateTime.Now;
            await LoadOccupiedBookingsAsync();
        }

        private async Task LoadOccupiedBookingsAsync()
        {
            try
            {
                btnRefresh.Enabled = false;
                _loadingBookings = true;
                var client = BaseApiClient.GetClient();
                using var response = await client.GetAsync("DatPhong");
                if (!response.IsSuccessStatusCode)
                    throw new InvalidOperationException($"API DatPhong trả về {(int)response.StatusCode}.");

                var all = await response.Content.ReadFromJsonAsync<List<BookingViewModel>>()
                    ?? new List<BookingViewModel>();
                _occupiedBookings = all.Where(b => b.TrangThai == "DangO" || b.TrangThai == "DaNhanPhong").ToList();

                cboOccupiedBookings.DataSource = null;
                cboOccupiedBookings.DisplayMember = "SoPhong";
                cboOccupiedBookings.ValueMember = "Id";
                cboOccupiedBookings.DataSource = _occupiedBookings;
                ClearInvoiceDetails();
            }
            catch (Exception ex)
            {
                _occupiedBookings.Clear();
                cboOccupiedBookings.DataSource = null;
                ClearInvoiceDetails();
                MessageBox.Show($"Lỗi tải danh sách đơn đang ở: {ex.Message}", "Lỗi API", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _loadingBookings = false;
                btnRefresh.Enabled = true;
            }

            if (cboOccupiedBookings.SelectedItem is BookingViewModel booking)
                await SelectBookingAsync(booking);
        }

        private async Task SelectBookingAsync(BookingViewModel booking)
        {
            _selectedBooking = booking;
            _servicesLoaded = false;
            _serviceDetails.Clear();
            dgvServices.DataSource = null;
            btnPrintInvoice.Enabled = false;
            txtCustomerName.Text = booking.TenKhachHang;
            txtRoomNumber.Text = booking.SoPhong;
            txtCheckIn.Text = booking.NgayNhanPhong == DateTime.MinValue ? "Chưa có" : booking.NgayNhanPhong.ToString("dd/MM/yyyy HH:mm");
            CalculateInvoiceTotals();

            try
            {
                var client = BaseApiClient.GetClient();
                using var response = await client.GetAsync($"SuDungDichVu/dat-phong/{booking.Id}");
                if (!response.IsSuccessStatusCode)
                    throw new InvalidOperationException($"API SuDungDichVu trả về {(int)response.StatusCode}.");
                var items = await response.Content.ReadFromJsonAsync<List<ChiTietDichVuViewModel>>()
                    ?? new List<ChiTietDichVuViewModel>();
                if (_selectedBooking?.Id != booking.Id) return;
                _serviceDetails = items;
                _servicesLoaded = true;
                dgvServices.DataSource = _serviceDetails;
                CalculateInvoiceTotals();
            }
            catch (Exception ex)
            {
                if (_selectedBooking?.Id != booking.Id) return;
                _servicesLoaded = false;
                CalculateInvoiceTotals();
                MessageBox.Show($"Không thể lấy tiền dịch vụ cho đơn #{booking.Id}: {ex.Message}", "Chưa đủ dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void CalculateInvoiceTotals()
        {
            if (_selectedBooking == null)
            {
                lblGrandTotalAmount.Text = "0 VNĐ";
                btnPrintInvoice.Enabled = false;
                return;
            }
            // API SuDungDichVu.Create đã cộng phí dịch vụ vào DatPhong.TongTien.
            // Vì vậy không cộng dịch vụ thêm một lần nữa, và không nhân TongTien với số ngày.
            // Đây là tổng đã ghi nhận trong đơn, không phải hóa đơn check-out cuối cùng.
            var booking = _selectedBooking;
            var start = booking.NgayNhan ?? DateTime.MinValue;
            var end = booking.NgayTra ?? DateTime.MinValue;
            var days = start != DateTime.MinValue && end > start
                ? Math.Max(1, (int)Math.Ceiling((end - start).TotalDays)) : 0;
            txtDays.Text = days > 0 ? $"{days} đêm theo đơn" : "Chưa đủ ngày";

            if (!_servicesLoaded || days == 0)
            {
                txtRoomRate.Text = "—";
                txtRoomTotal.Text = "—";
                txtServiceTotal.Text = "—";
                lblGrandTotalAmount.Text = "Chưa đủ dữ liệu";
                btnPrintInvoice.Enabled = false;
                return;
            }
            decimal serviceTotal = _serviceDetails.Sum(s => s.ThanhTien);
            decimal roomTotal = booking.TongTien - serviceTotal;
            bool valid = roomTotal >= 0;
            txtRoomRate.Text = valid ? $"{roomTotal / days:N0} VNĐ/đêm (ước tính)" : "—";
            txtRoomTotal.Text = valid ? $"{roomTotal:N0} VNĐ" : "—";
            txtServiceTotal.Text = $"{serviceTotal:N0} VNĐ";
            lblGrandTotalAmount.Text = valid ? $"{booking.TongTien:N0} VNĐ" : "Dữ liệu không nhất quán";
            btnPrintInvoice.Enabled = valid;
        }

        private void btnPay_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chưa hỗ trợ thanh toán/check-out. API phải lưu thanh toán, " +
                "kiểm tra số tiền và cập nhật trạng thái đơn/phòng trong cùng giao dịch.",
                "Chờ tích hợp API", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void btnPrintInvoice_Click(object sender, EventArgs e)
        {
            if (_selectedBooking == null || !_servicesLoaded || !btnPrintInvoice.Enabled) return;
            var sb = new StringBuilder();
            sb.AppendLine("BẢNG TÍNH TẠM — KHÔNG PHẢI HÓA ĐƠN THANH TOÁN");
            sb.AppendLine($"Mã đơn: {_selectedBooking.Id}");
            sb.AppendLine($"Khách: {_selectedBooking.TenKhachHang}");
            sb.AppendLine($"Phòng: {_selectedBooking.SoPhong}");
            sb.AppendLine($"Số đêm theo đơn: {txtDays.Text}");
            sb.AppendLine($"Tiền phòng: {txtRoomTotal.Text}");
            sb.AppendLine("Dịch vụ:");
            foreach (var s in _serviceDetails)
                sb.AppendLine($" • {s.TenDichVu} x{s.SoLuong}: {s.ThanhTien:N0} VNĐ");
            sb.AppendLine($"Tiền dịch vụ: {txtServiceTotal.Text}");
            sb.AppendLine($"Tổng đang ghi nhận: {lblGrandTotalAmount.Text}");
            sb.AppendLine("Chưa bao gồm điều chỉnh khi trả phòng và chưa xác nhận thanh toán.");
            MessageBox.Show(sb.ToString(), "Bảng tính tạm", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void cboOccupiedBookings_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loadingBookings) return;
            if (cboOccupiedBookings.SelectedItem is BookingViewModel booking)
                await SelectBookingAsync(booking);
        }

        private void dtpCheckOut_ValueChanged(object sender, EventArgs e)
        {
            // Ngày thực tế phải xử lý trong quy trình check-out API, không tự chỉnh tiền đã chốt.
        }

        private void ClearInvoiceDetails()
        {
            _selectedBooking = null;
            _servicesLoaded = false;
            _serviceDetails.Clear();
            txtCustomerName.Clear();
            txtRoomNumber.Clear();
            txtCheckIn.Clear();
            txtDays.Clear();
            txtRoomRate.Clear();
            txtRoomTotal.Clear();
            txtServiceTotal.Clear();
            lblGrandTotalAmount.Text = "0 VNĐ";
            dgvServices.DataSource = null;
            btnPrintInvoice.Enabled = false;
        }

        private async void btnRefresh_Click(object sender, EventArgs e) => await LoadOccupiedBookingsAsync();
    }
}
