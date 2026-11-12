using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Printing;
using System.Net.Http.Json;
using System.Windows.Forms;
using HotelManagement.Desktop.Models;
using HotelManager.Services;

namespace HotelManager.UI
{
    public partial class FrmManageInvoices : Form
    {
        private BookingViewModel? _selectedBooking;
        private List<ChiTietDichVuViewModel> _services = new();
        private int _invoiceLoadVersion;
        private readonly int? _preferredBookingId;

        public FrmManageInvoices(int? preferredBookingId = null)
        {
            InitializeComponent();
            _preferredBookingId = preferredBookingId;
        }

        private async void FrmManageInvoices_Load(object sender, EventArgs e)
        {
            SetupInvoiceGrid();
            txtCustomerName.ReadOnly = true; txtRoomNumber.ReadOnly = true;
            txtCheckIn.ReadOnly = true; txtRoomTotal.ReadOnly = true;
            txtServiceTotal.ReadOnly = true; txtRoomRate.ReadOnly = true;
            txtDays.ReadOnly = true;
            dtpCheckOut.Enabled = false; // Ngày trả của đơn không chỉnh tùy ý từ màn hình thanh toán.
            await LoadDanhSachDonDatAsync();
        }

        private void SetupInvoiceGrid()
        {
            dgvServices.AutoGenerateColumns = false;
            dgvServices.Columns.Clear();
            dgvServices.ReadOnly = true;
            dgvServices.AllowUserToAddRows = false;
            dgvServices.RowHeadersVisible = false;
            dgvServices.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvServices.BackgroundColor = Color.White;
            void Add(string field, string header, string? format = null)
            {
                var col = new DataGridViewTextBoxColumn { DataPropertyName = field, HeaderText = header };
                if (format != null) col.DefaultCellStyle.Format = format;
                dgvServices.Columns.Add(col);
            }
            Add("TenDichVu", "Tên dịch vụ");
            Add("SoLuong", "Số lượng");
            Add("DonGia", "Đơn giá", "N0");
            Add("ThanhTien", "Thành tiền", "N0");
        }

        private async System.Threading.Tasks.Task LoadDanhSachDonDatAsync()
        {
            try
            {
                var client = BaseApiClient.GetClient();
                var list = await client.GetFromJsonAsync<List<BookingViewModel>>("DatPhong");

                // Chỉ đơn đã nhận phòng mới được thanh toán/trả phòng.
                var occupiedList = list?.FindAll(b => b.TrangThai == "DaNhanPhong") ?? new List<BookingViewModel>();
                _selectedBooking = null;
                _services.Clear();
                dgvServices.DataSource = null;
                cboOccupiedBookings.DataSource = null;
                cboOccupiedBookings.DataSource = occupiedList;
                cboOccupiedBookings.DisplayMember = "DisplayBooking";
                cboOccupiedBookings.ValueMember = "Id";
                if (_preferredBookingId.HasValue)
                {
                    var match = occupiedList.FirstOrDefault(b => b.Id == _preferredBookingId.Value);
                    if (match != null) cboOccupiedBookings.SelectedItem = match;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách đơn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void cboOccupiedBookings_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboOccupiedBookings.SelectedItem is BookingViewModel booking)
            {
                _selectedBooking = booking;
                HienThiThongTinDon(booking);
                await LoadServicesForBookingAsync(booking);
            }
        }

        private void HienThiThongTinDon(BookingViewModel booking)
        {
            // Hiển thị dữ liệu lên giao diện nếu có các Control tương ứng
            txtCustomerName.Text = booking.TenKhachHang;
            txtRoomNumber.Text = booking.SoPhong;
            txtCheckIn.Text = booking.NgayNhan?.ToString("dd/MM/yyyy") ?? "";
            if (booking.NgayTra.HasValue) dtpCheckOut.Value = booking.NgayTra.Value;
            txtDays.Text = (booking.NgayNhan.HasValue && booking.NgayTra.HasValue)
                ? Math.Max(1, (booking.NgayTra.Value.Date - booking.NgayNhan.Value.Date).Days).ToString() : "";
            txtRoomRate.Text = string.Join(", ", booking.DanhSachPhong.Select(r => r.GiaPhong.ToString("N0")));
            txtRoomTotal.Text = booking.TongTien.ToString("N0");
            txtServiceTotal.Text = "Đang tải...";
            lblGrandTotalAmount.Text = booking.TongTien.ToString("N0") + " VNĐ";
        }

        private async System.Threading.Tasks.Task LoadServicesForBookingAsync(BookingViewModel booking)
        {
            int version = ++_invoiceLoadVersion;
            try
            {
                var list = await BaseApiClient.GetClient().GetFromJsonAsync<List<ChiTietDichVuViewModel>>(
                    $"SuDungDichVu/dat-phong/{booking.Id}") ?? new();
                if (version != _invoiceLoadVersion || _selectedBooking?.Id != booking.Id) return;
                _services = list;
                dgvServices.DataSource = null;
                dgvServices.DataSource = list;
                decimal serviceTotal = list.Sum(x => x.ThanhTien);
                // DatPhong.TongTien đã bao gồm tiền dịch vụ từ API; không được cộng hai lần.
                decimal roomTotal = booking.TongTien - serviceTotal;
                txtRoomTotal.Text = roomTotal.ToString("N0");
                txtServiceTotal.Text = serviceTotal.ToString("N0");
                lblGrandTotalAmount.Text = booking.TongTien.ToString("N0") + " VNĐ";
            }
            catch (Exception ex)
            {
                if (version != _invoiceLoadVersion) return;
                txtServiceTotal.Text = "Không tải được";
                MessageBox.Show("Không tải được tiền dịch vụ. Chưa nên thanh toán khi chưa đối chiếu: " + ex.Message);
            }
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await LoadDanhSachDonDatAsync();
        }

        private void dtpCheckOut_ValueChanged(object sender, EventArgs e)
        {
            // Tự động tính lại số ngày/tiền phòng nếu khách thay đổi ngày trả
        }

        private async void btnPay_Click(object sender, EventArgs e)
        {
            if (_selectedBooking == null || _selectedBooking.TrangThai != "DaNhanPhong")
            {
                MessageBox.Show("Chỉ thanh toán đơn đã nhận phòng.", "Cảnh báo");
                return;
            }
            if (txtServiceTotal.Text == "Không tải được" || txtServiceTotal.Text == "Đang tải...")
            {
                MessageBox.Show("Cần tải được tiền dịch vụ trước khi thanh toán.");
                return;
            }
            if (MessageBox.Show($"Xác nhận thanh toán {_selectedBooking.TongTien:N0} VNĐ " +
                $"và trả phòng cho đơn #{_selectedBooking.Id}?", "Xác nhận thanh toán",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            btnPay.Enabled = false;
            try
            {
                using var response = await BaseApiClient.GetClient().PostAsJsonAsync(
                    $"ThanhToan/check-out/{_selectedBooking.Id}", new { PhuongThuc = "TienMat" });
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Thanh toán không thành công (HTTP {(int)response.StatusCode}): {error}",
                        "Lỗi API", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                MessageBox.Show("Thanh toán và trả phòng thành công.", "Thông báo");
                await LoadDanhSachDonDatAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi thanh toán: {ex.Message}", "Lỗi kết nối");
            }
            finally
            {
                btnPay.Enabled = true;
            }
        }

        private void btnPrintInvoice_Click(object sender, EventArgs e)
        {
            if (_selectedBooking == null) return;
            MessageBox.Show("Bản in này là bảng kê tạm tính, không thay thế chứng từ thanh toán.");

            PrintDocument pd = new PrintDocument();
            pd.PrintPage += (s, ev) =>
            {
                Graphics g = ev.Graphics!;
                Font titleFont = new Font("Arial", 16, FontStyle.Bold);
                Font bodyFont = new Font("Arial", 11, FontStyle.Regular);

                g.DrawString("BẢNG KÊ TIỀN PHÒNG (TẠM TÍNH)", titleFont, Brushes.Black, new PointF(100, 20));
                g.DrawString($"Mã đơn: {_selectedBooking.Id}", bodyFont, Brushes.Black, new PointF(30, 70));
                g.DrawString($"Khách hàng: {_selectedBooking.TenKhachHang}", bodyFont, Brushes.Black, new PointF(30, 100));
                g.DrawString($"Phòng: {_selectedBooking.SoPhong}", bodyFont, Brushes.Black, new PointF(30, 130));
                g.DrawString($"Tiền phòng: {txtRoomTotal.Text} VNĐ", bodyFont, Brushes.Black, new PointF(30, 170));
                g.DrawString($"Tiền dịch vụ: {txtServiceTotal.Text} VNĐ", bodyFont, Brushes.Black, new PointF(30, 200));
                g.DrawString($"Tổng cộng: {_selectedBooking.TongTien:N0} VNĐ", titleFont, Brushes.Black, new PointF(30, 250));
            };

            PrintPreviewDialog preview = new PrintPreviewDialog { Document = pd };
            preview.ShowDialog();
        }
    }
}