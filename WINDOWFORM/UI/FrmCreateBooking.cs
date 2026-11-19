using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows.Forms;
using HotelManagement.Desktop.Models;
using HotelManager.Services;

namespace HotelManager.UI
{
    public partial class FrmCreateBooking : Form
    {
        public FrmCreateBooking()
        {
            InitializeComponent();
        }

        private async void FrmCreateBooking_Load(object sender, EventArgs e)
        {
            dtpCheckIn.Value = DateTime.Today;
            dtpCheckOut.Value = DateTime.Today.AddDays(1);

            // Bắt sự kiện thay đổi ngày để load lại danh sách phòng trống
            dtpCheckIn.ValueChanged += async (s, ev) => await LoadPhongTrongAsync();
            dtpCheckOut.ValueChanged += async (s, ev) => await LoadPhongTrongAsync();

            await LoadPhongTrongAsync();
        }

        private async System.Threading.Tasks.Task LoadPhongTrongAsync()
        {
            try
            {
                if (dtpCheckOut.Value.Date <= dtpCheckIn.Value.Date)
                {
                    cboRooms.DataSource = null;
                    return;
                }
                var tuNgay = dtpCheckIn.Value.ToString("yyyy-MM-dd");
                var denNgay = dtpCheckOut.Value.ToString("yyyy-MM-dd");

                var client = BaseApiClient.GetClient(); // BaseAddress = "https://localhost:7203/api/"
                var phongTrongs = await client.GetFromJsonAsync<List<PhongViewModel>>(
                    $"Phong/tim-kiem?ngayNhan={tuNgay}&ngayTra={denNgay}");

                cboRooms.DataSource = phongTrongs;
                cboRooms.DisplayMember = "SoPhong";
                cboRooms.ValueMember = "PhongId";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể tải danh sách phòng trống: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            var ten = txtCustomerName.Text.Trim();
            var sdt = txtPhone.Text.Trim();
            if (string.IsNullOrWhiteSpace(ten) || string.IsNullOrWhiteSpace(sdt))
            {
                MessageBox.Show("Vui lòng nhập tên và số điện thoại khách hàng.");
                return;
            }
            if (cboRooms.SelectedValue is not int phongId || phongId <= 0)
            {
                MessageBox.Show("Vui lòng chọn phòng trống.");
                return;
            }
            if (dtpCheckIn.Value.Date < DateTime.Today ||
                dtpCheckOut.Value.Date <= dtpCheckIn.Value.Date)
            {
                MessageBox.Show("Ngày nhận/trả không hợp lệ.");
                return;
            }
            btnSave.Enabled = false;
            try
            {
                var client = BaseApiClient.GetClient();
                // Tìm chính xác theo số điện thoại để không tạo trùng hồ sơ.
                var dsKhach = await client.GetFromJsonAsync<List<CustomerDto>>(
                    $"KhachHang/tim-kiem?tuKhoa={Uri.EscapeDataString(sdt)}") ?? new();
                var khach = dsKhach.FirstOrDefault(k =>
                    string.Equals(k.PhoneNumber?.Trim(), sdt, StringComparison.Ordinal));
                int khachHangId;
                if (khach != null)
                {
                    khachHangId = khach.CustomerId;
                    if (!string.Equals(khach.FullName?.Trim(), ten, StringComparison.OrdinalIgnoreCase))
                    {
                        var confirm = MessageBox.Show(
                            $"Số điện thoại đã thuộc khách '{khach.FullName}'. Dùng hồ sơ đã có?",
                            "Kiểm tra khách hàng", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (confirm != DialogResult.Yes) return;
                    }
                }
                else
                {
                    using var taoKhach = await client.PostAsJsonAsync("KhachHang",
                        new { HoTen = ten, SoDienThoai = sdt });
                    if (!taoKhach.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"Không thể tạo khách hàng: HTTP {(int)taoKhach.StatusCode}.");
                        return;
                    }
                    var moi = await taoKhach.Content.ReadFromJsonAsync<CustomerDto>();
                    if (moi == null || moi.CustomerId <= 0)
                    {
                        MessageBox.Show("API không trả mã khách hàng hợp lệ.");
                        return;
                    }
                    khachHangId = moi.CustomerId;
                }
                using var response = await client.PostAsJsonAsync("DatPhong", new
                {
                    KhachHangId = khachHangId,
                    DanhSachPhongId = new[] { phongId },
                    NgayNhan = dtpCheckIn.Value.Date,
                    NgayTra = dtpCheckOut.Value.Date
                });
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Không tạo được đơn (HTTP {(int)response.StatusCode}): {error}");
                    return;
                }
                MessageBox.Show("Đặt phòng tại quầy thành công, đơn đang chờ xác nhận.");
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi đặt phòng tại quầy: {ex.Message}");
            }
            finally
            {
                btnSave.Enabled = true;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}