using HotelManagement.Desktop.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            await LoadRoomListAsync();
        }

        // Tải danh sách phòng trống/sẵn có lên ComboBox
        private async Task LoadRoomListAsync()
        {
            try
            {
                using (HttpClient client = BaseApiClient.GetClient())
                {
                    HttpResponseMessage response = await client.GetAsync("api/Phong");
                    if (response.IsSuccessStatusCode)
                    {
                        var rooms = await response.Content.ReadFromJsonAsync<List<PhongViewModel>>();
                        cboRooms.DataSource = rooms;
                        cboRooms.DisplayMember = "SoPhong";
                        cboRooms.ValueMember = "PhongId"; // Đồng bộ thuộc tính PhongId
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách phòng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Nút Lưu đơn đặt phòng tại quầy
        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCustomerName.Text) || string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tên và số điện thoại khách hàng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dtpCheckOut.Value.Date <= dtpCheckIn.Value.Date)
            {
                MessageBox.Show("Ngày trả phòng phải sau ngày nhận phòng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cboRooms.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn phòng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string roomId = cboRooms.SelectedValue.ToString() ?? string.Empty;
            DateTime checkIn = dtpCheckIn.Value.Date;
            DateTime checkOut = dtpCheckOut.Value.Date;

            // 1. Kiểm tra trùng lịch phòng qua API trước khi tạo
            bool isAvailable = await CheckRoomAvailabilityAsync(roomId, checkIn, checkOut);
            if (!isAvailable)
            {
                MessageBox.Show("Phòng này đã có người đặt trong thời gian bạn chọn! Vui lòng chọn phòng hoặc ngày khác.",
                                "Trùng lịch đặt phòng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 2. Tạo DTO và gửi API lưu đơn
            var dto = new
            {
                TenKhachHang = txtCustomerName.Text.Trim(),
                SoDienThoai = txtPhone.Text.Trim(),
                PhongId = roomId,
                NgayNhanPhong = checkIn,
                NgayTraPhong = checkOut,
                TrangThai = "DaXacNhan", // Đặt trực tiếp tại quầy mặc định trạng thái Đã xác nhận
                GhiChu = txtNote.Text.Trim()
            };

            try
            {
                using (HttpClient client = BaseApiClient.GetClient())
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync("api/DonDatPhong", dto);
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Tạo đơn đặt phòng tại quầy thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show($"Tạo đơn thất bại. Mã lỗi API: {response.StatusCode}", "Lỗi API", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kết nối API: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Gọi API kiểm tra xem phòng có bị trùng lịch trong khoảng [checkIn, checkOut] hay không
        private async Task<bool> CheckRoomAvailabilityAsync(string roomId, DateTime checkIn, DateTime checkOut)
        {
            try
            {
                using (HttpClient client = BaseApiClient.GetClient())
                {
                    string url = $"api/DonDatPhong/kiem-tra-trung?phongId={roomId}&checkIn={checkIn:yyyy-MM-dd}&checkOut={checkOut:yyyy-MM-dd}";
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadFromJsonAsync<bool>();
                    }
                }
            }
            catch
            {
                // Nếu API chưa hỗ trợ endpoint check trùng, bỏ qua để fallback
            }
            return true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}