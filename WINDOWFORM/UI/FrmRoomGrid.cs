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
            // Tên FlowLayoutPanel THỰC TẾ trong FrmRoomGrid.Designer.cs là flpRooms.
            // Bản cũ tìm nhầm tên rồi return im lặng, khiến sơ đồ luôn trắng.
            FlowLayoutPanel flp = flpRooms;

            List<PhongViewModel>? rooms;
            try
            {
                var client = BaseApiClient.GetClient();
                rooms = await client.GetFromJsonAsync<List<PhongViewModel>>("Phong");
                if (rooms == null) throw new InvalidOperationException("API không trả danh sách phòng.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không tải được sơ đồ phòng: {ex.Message}", "Lỗi API");
                return;
            }
            flp.Dock = DockStyle.None;
            flp.SetBounds(12, 60, ClientSize.Width - 24, ClientSize.Height - 72);
            flp.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            flp.AutoScroll = true;
            flp.BackColor = Color.FromArgb(245, 247, 250);
            flp.Padding = new Padding(10);
            pnlLegend.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlMaintenance.BackColor = Color.LightGray;
            pnlOccupied.BackColor = Color.LightCoral;
            pnlAvailable.BackColor = Color.LightGreen;
            var reservedColor = new Panel { Location = new Point(410, 10), Size = new Size(20, 20), BackColor = Color.Khaki };
            var reservedLabel = new Label { Location = new Point(438, 12), AutoSize = true, Text = "Đã đặt" };
            if (!pnlLegend.Controls.ContainsKey("reservedLegend"))
            {
                reservedColor.Name = "reservedLegend";
                pnlLegend.Controls.Add(reservedColor);
                pnlLegend.Controls.Add(reservedLabel);
            }
            flp.SuspendLayout();
            flp.Controls.Clear();
            if (rooms.Count == 0)
            {
                flp.Controls.Add(new Label { AutoSize = true, Text = "Chưa có phòng để hiển thị.", Padding = new Padding(12) });
            }
            foreach (var r in rooms)
            {
                Button btn = new Button
                {
                    Width = 156,
                    Height = 106,
                    Text = $"Phòng {r.SoPhong}\n{r.TenLoaiPhong}\n[{TrangThaiTiengViet.Phong(r.TrangThai)}]",
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    Tag = r,
                    Margin = new Padding(8),
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand
                };

                switch (r.TrangThai)
                {
                    case "DangSuDung":
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

                btn.FlatAppearance.BorderColor = Color.FromArgb(210, 216, 220);
                btn.Click += (_, _) => MessageBox.Show(
                    $"Phòng {r.SoPhong}\nLoại: {r.TenLoaiPhong}\nGiá/đêm: {r.GiaPhong:N0} VNĐ\nTrạng thái: {TrangThaiTiengViet.Phong(r.TrangThai)}",
                    "Thông tin phòng", MessageBoxButtons.OK, MessageBoxIcon.Information);
                flp.Controls.Add(btn);
            }
            flp.ResumeLayout(true);
        }
    }
}