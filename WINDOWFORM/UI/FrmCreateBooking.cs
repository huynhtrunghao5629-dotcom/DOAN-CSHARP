using System;
using System.Collections.Generic;
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
            dtpCheckIn.Value = DateTime.Now;
            dtpCheckOut.Value = DateTime.Now.AddDays(1);

            // Bắt sự kiện thay đổi ngày để load lại danh sách phòng trống
            dtpCheckIn.ValueChanged += async (s, ev) => await LoadPhongTrongAsync();
            dtpCheckOut.ValueChanged += async (s, ev) => await LoadPhongTrongAsync();

            await LoadPhongTrongAsync();
        }

        private async System.Threading.Tasks.Task LoadPhongTrongAsync()
        {
            try
            {
                var tuNgay = dtpCheckIn.Value.ToString("yyyy-MM-dd");
                var denNgay = dtpCheckOut.Value.ToString("yyyy-MM-dd");

                var client = BaseApiClient.GetClient(); // BaseAddress = "https://localhost:7203/api/"
                var phongTrongs = await client.GetFromJsonAsync<List<PhongViewModel>>($"Phong/phong-trong?tuNgay={tuNgay}&denNgay={denNgay}");

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
            if (string.IsNullOrWhiteSpace(txtCustomerName.Text) || string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Vui lòng nhập tên khách hàng và số điện thoại!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cboRooms.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn phòng trống!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dtpCheckOut.Value <= dtpCheckIn.Value)
            {
                MessageBox.Show("Ngày trả phòng phải lớn hơn ngày nhận phòng!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Nếu ngày nhận phòng là hôm nay -> Khách nhận phòng ngay (DaNhanPhong), ngược lại là Đã đặt (DaDat)
            string trangThai = (dtpCheckIn.Value.Date == DateTime.Today) ? "DaNhanPhong" : "DaDat";

            var req = new
            {
                TenKhachHang = txtCustomerName.Text.Trim(),
                SoDienThoai = txtPhone.Text.Trim(),
                PhongId = (int)cboRooms.SelectedValue,
                NgayNhan = dtpCheckIn.Value,
                NgayTra = dtpCheckOut.Value,
                GhiChu = txtNote.Text.Trim(),
                TrangThai = trangThai
            };

            var client = BaseApiClient.GetClient();
            var res = await client.PostAsJsonAsync("DatPhong", req);

            if (res.IsSuccessStatusCode)
            {
                MessageBox.Show($"Tạo đơn đặt phòng thành công! (Trạng thái: {TrangThaiTiengViet.Don(trangThai)})", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Tạo đơn đặt phòng thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}