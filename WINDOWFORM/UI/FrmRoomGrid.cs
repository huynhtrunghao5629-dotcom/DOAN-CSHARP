using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http.Json;
using System.Windows.Forms;
using HotelManagement.Desktop.Models;
using HotelManager.Services;

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
            await LoadRoomGridAsync();
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await LoadRoomGridAsync();
        }

        public async System.Threading.Tasks.Task LoadRoomGridAsync()
        {
            // Tự động tìm FlowLayoutPanel trên giao diện
            FlowLayoutPanel? flp = Controls.Find("flpRoomContainer", true).Length > 0
                ? Controls.Find("flpRoomContainer", true)[0] as FlowLayoutPanel
                : (Controls.Find("flpSoDoPhong", true).Length > 0 ? Controls.Find("flpSoDoPhong", true)[0] as FlowLayoutPanel : null);

            if (flp == null) return;

            flp.Controls.Clear();
            var client = BaseApiClient.GetClient();
            var rooms = await client.GetFromJsonAsync<List<PhongViewModel>>("Phong");

            if (rooms == null) return;

            foreach (var r in rooms)
            {
                Button btn = new Button
                {
                    Width = 115,
                    Height = 90,
                    Text = $"Phòng {r.SoPhong}\n{r.TenLoaiPhong}\n[{TrangThaiTiengViet.Phong(r.TrangThai)}]",
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    Tag = r,
                    Margin = new Padding(6)
                };

                switch (r.TrangThai)
                {
                    case "DaNhanPhong":
                    case "DangO":
                        btn.BackColor = Color.LightCoral;
                        btn.ForeColor = Color.White;
                        break;
                    case "DaDat":
                        btn.BackColor = Color.Khaki;
                        break;
                    case "DangBaoTri":
                        btn.BackColor = Color.LightGray;
                        break;
                    default:
                        btn.BackColor = Color.LightGreen;
                        break;
                }

                flp.Controls.Add(btn);
            }
        }
    }
}