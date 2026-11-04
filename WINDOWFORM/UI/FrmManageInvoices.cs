using System;
using System.Collections.Generic;
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

        public FrmManageInvoices()
        {
            InitializeComponent();
        }

        private async void FrmManageInvoices_Load(object sender, EventArgs e)
        {
            await LoadDanhSachDonDatAsync();
        }

        private async System.Threading.Tasks.Task LoadDanhSachDonDatAsync()
        {
            try
            {
                var client = BaseApiClient.GetClient();
                var list = await client.GetFromJsonAsync<List<BookingViewModel>>("DatPhong");

                // Lấy các đơn đang ở hoặc đã đặt để làm thủ tục thanh toán
                var occupiedList = list?.FindAll(b => b.TrangThai == "DaNhanPhong" || b.TrangThai == "DaDat") ?? new List<BookingViewModel>();

                cboOccupiedBookings.DataSource = occupiedList;
                cboOccupiedBookings.DisplayMember = "TenKhachHang";
                cboOccupiedBookings.ValueMember = "Id";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách đơn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cboOccupiedBookings_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboOccupiedBookings.SelectedItem is BookingViewModel booking)
            {
                _selectedBooking = booking;
                HienThiThongTinDon(booking);
            }
        }

        private void HienThiThongTinDon(BookingViewModel booking)
        {
            // Hiển thị dữ liệu lên giao diện nếu có các Control tương ứng
            if (Controls.Find("txtTongTien", true).Length > 0)
                Controls.Find("txtTongTien", true)[0].Text = booking.TongTien.ToString("N0");
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
            if (_selectedBooking == null)
            {
                MessageBox.Show("Vui lòng chọn đơn đặt phòng cần thanh toán!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var client = BaseApiClient.GetClient();
                var thanhToanDto = new
                {
                    DatPhongId = _selectedBooking.Id,
                    NgayThanhToan = DateTime.Now,
                    TongTien = _selectedBooking.TongTien,
                    PhuongThuc = "TienMat"
                };

                await client.PostAsJsonAsync("ThanhToan", thanhToanDto);
                await client.PutAsJsonAsync($"DatPhong/{_selectedBooking.Id}/trang-thai", new { TrangThai = "DaTraPhong" });

                MessageBox.Show("Thanh toán & Check-out thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadDanhSachDonDatAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi thanh toán: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPrintInvoice_Click(object sender, EventArgs e)
        {
            if (_selectedBooking == null) return;

            PrintDocument pd = new PrintDocument();
            pd.PrintPage += (s, ev) =>
            {
                Graphics g = ev.Graphics!;
                Font titleFont = new Font("Arial", 16, FontStyle.Bold);
                Font bodyFont = new Font("Arial", 11, FontStyle.Regular);

                g.DrawString("HÓA ĐƠN THANH TOÁN KHÁCH SẠN", titleFont, Brushes.Black, new PointF(100, 20));
                g.DrawString($"Mã đơn: {_selectedBooking.Id}", bodyFont, Brushes.Black, new PointF(30, 70));
                g.DrawString($"Khách hàng: {_selectedBooking.TenKhachHang}", bodyFont, Brushes.Black, new PointF(30, 100));
                g.DrawString($"Phòng: {_selectedBooking.SoPhong}", bodyFont, Brushes.Black, new PointF(30, 130));
                g.DrawString($"Tổng tiền: {_selectedBooking.TongTien:N0} VNĐ", titleFont, Brushes.Red, new PointF(30, 180));
            };

            PrintPreviewDialog preview = new PrintPreviewDialog { Document = pd };
            preview.ShowDialog();
        }
    }
}