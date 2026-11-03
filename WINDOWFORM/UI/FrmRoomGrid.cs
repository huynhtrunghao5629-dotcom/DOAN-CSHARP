using HotelManager.Services;
using HotelManagement.Desktop.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HotelManager.UI
{
    public partial class FrmRoomGrid : Form
    {
        public FrmRoomGrid()
        {
            InitializeComponent();
        }

        private async void FrmRoomGrid_Load(object sender, EventArgs e)
        {
            await RenderRoomGridAsync();
        }

        private async Task RenderRoomGridAsync()
        {
            flpRooms.Controls.Clear();

            try
            {
                {
                HttpClient client = BaseApiClient.GetClient();
                    HttpResponseMessage response = await client.GetAsync("Phong");
                    if (response.IsSuccessStatusCode)
                    {
                        var rooms = await response.Content.ReadFromJsonAsync<List<PhongViewModel>>();
                        if (rooms != null)
                        {
                            foreach (var room in rooms)
                            {
                                Button btnRoom = CreateRoomButton(room);
                                flpRooms.Controls.Add(btnRoom);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Không thể lấy dữ liệu sơ đồ phòng. Mã lỗi: {response.StatusCode}", "Lỗi API", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kết nối API sơ đồ phòng: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Button CreateRoomButton(PhongViewModel room)
        {
            Button btn = new Button
            {
                Width = 125,
                Height = 100,
                Margin = new Padding(10),
                Tag = room,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };

            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.BorderColor = Color.DarkGray;

            // Tách mã lưu trong API và nhãn tiếng Việt trên giao diện.
            string statusText = TrangThaiTiengViet.Phong(room.TrangThai);
            switch (statusText)
            {
                case "Đang có khách":
                    btn.BackColor = Color.FromArgb(220, 53, 69);
                    btn.ForeColor = Color.White;
                    break;
                case "Đang bảo trì":
                case "Đang dọn phòng":
                case "Đã đặt":
                    btn.BackColor = Color.FromArgb(255, 193, 7);
                    btn.ForeColor = Color.Black;
                    break;
                case "Trống":
                    btn.BackColor = Color.FromArgb(40, 167, 69);
                    btn.ForeColor = Color.White;
                    break;
                default:
                    // Không hiểu trạng thái thì không đánh dấu nhầm là phòng trống.
                    btn.BackColor = Color.LightGray;
                    btn.ForeColor = Color.Black;
                    break;
            }

            btn.Text = $"PHÒNG {room.SoPhong}\n({room.TenLoaiPhong})\n\n[{statusText}]";
            btn.Click += RoomButton_Click;

            return btn;
        }

        private void RoomButton_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn && btn.Tag is PhongViewModel room)
            {
                if (TrangThaiTiengViet.Phong(room.TrangThai) == "Trống")
                {
                    var result = MessageBox.Show($"Phòng {room.SoPhong} đang TRỐNG.\nBạn có muốn mở màn hình Đặt phòng tại quầy không?",
                                                 "Thao tác nhanh", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        FrmCreateBooking frm = new FrmCreateBooking();
                        if (frm.ShowDialog() == DialogResult.OK)
                        {
                            _ = RenderRoomGridAsync();
                        }
                    }
                }
                else
                {
                    MessageBox.Show($"Phòng: {room.SoPhong}\nLoại: {room.TenLoaiPhong}\nGiá: {room.GiaPhong:N0} VNĐ\nTrạng thái: {TrangThaiTiengViet.Phong(room.TrangThai)}",
                                    "Thông tin chi tiết phòng", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await RenderRoomGridAsync();
        }
    }
}