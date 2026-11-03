using HotelManager.Services;
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
                {
                HttpClient client = BaseApiClient.GetClient();
                    HttpResponseMessage response = await client.GetAsync("Phong");
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

        // API hiện yêu cầu KhachHangId, DanhSachPhongId, NgayNhan, NgayTra.
        // Chưa có bước tra cứu/xác minh khách hàng và chống trùng tại thời điểm tạo.
        // Tạm khóa lưu để tránh tạo dữ liệu sai hoặc đánh lừa người dùng.
        private void btnSave_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Chưa thể lưu đơn tại quầy: cần API hỗ trợ tra cứu/tạo khách hàng, " +
                "dữ liệu KhachHangId + DanhSachPhongId và kiểm tra trùng lịch ngay khi lưu.",
                "Chức năng chờ tích hợp", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}