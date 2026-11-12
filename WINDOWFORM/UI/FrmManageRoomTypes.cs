using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using HotelManager.Services;

namespace HotelManager.UI
{
    // Quản lý loại phòng hoàn toàn bằng WinForms; sử dụng API LoaiPhong có sẵn.
    // API hiện tại chỉ cập nhật TenLoai và GiaPhong khi PUT.
    public sealed class FrmManageRoomTypes : Form
    {
        private sealed class RoomTypeItem
        {
            public int LoaiPhongId { get; set; }
            public string TenLoai { get; set; } = "";
            public decimal GiaPhong { get; set; }
            public int SoNguoiToiDa { get; set; }
            public string? MoTa { get; set; }
        }
        private readonly DataGridView grid = new();
        private readonly TextBox nameBox = new();
        private readonly NumericUpDown priceBox = new();
        private readonly NumericUpDown capacityBox = new();
        private readonly TextBox descriptionBox = new();
        private readonly Label info = new();
        private RoomTypeItem? selected;
        private bool busy;

        public FrmManageRoomTypes()
        {
            Text = "Quản lý loại phòng và bảng giá";
            ClientSize = new Size(940, 570);
            MinimumSize = new Size(800, 500);
            Font = new Font("Segoe UI", 9.5F);
            StartPosition = FormStartPosition.CenterParent;
            var panel = new Panel { Dock = DockStyle.Top, Height = 172, BackColor = Color.FromArgb(245, 247, 250) };
            Controls.Add(grid);
            Controls.Add(panel);
            grid.Dock = DockStyle.Fill;
            grid.AutoGenerateColumns = false;
            grid.AllowUserToAddRows = false;
            grid.ReadOnly = true;
            grid.MultiSelect = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.BackgroundColor = Color.White;
            grid.RowHeadersVisible = false;
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "LoaiPhongId", HeaderText = "Mã", FillWeight = 40 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TenLoai", HeaderText = "Tên loại phòng", FillWeight = 130 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "GiaPhong", HeaderText = "Giá / đêm (VNĐ)", FillWeight = 110, DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" } });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SoNguoiToiDa", HeaderText = "Sức chứa", FillWeight = 65 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "MoTa", HeaderText = "Mô tả", FillWeight = 160 });
            grid.SelectionChanged += (_, _) => SelectItem();
            void label(string caption, int x, int y) => panel.Controls.Add(new Label { Text = caption, AutoSize = true, Location = new Point(x, y + 5) });
            label("Tên loại phòng", 18, 18); nameBox.SetBounds(130, 18, 230, 27); panel.Controls.Add(nameBox);
            label("Giá / đêm", 390, 18); priceBox.SetBounds(475, 18, 180, 27); priceBox.Maximum = 1000000000; priceBox.DecimalPlaces = 0; priceBox.Increment = 50000; priceBox.ThousandsSeparator = true; panel.Controls.Add(priceBox);
            label("Sức chứa", 685, 18); capacityBox.SetBounds(770, 18, 100, 27); capacityBox.Minimum = 1; capacityBox.Maximum = 50; panel.Controls.Add(capacityBox);
            label("Mô tả", 18, 59); descriptionBox.SetBounds(130, 59, 740, 27); panel.Controls.Add(descriptionBox);
            var add = new Button { Text = "Thêm mới", Bounds = new Rectangle(130, 100, 105, 31) };
            var save = new Button { Text = "Lưu", Bounds = new Rectangle(245, 100, 105, 31) };
            var delete = new Button { Text = "Xóa", Bounds = new Rectangle(360, 100, 105, 31) };
            var refresh = new Button { Text = "Tải lại", Bounds = new Rectangle(475, 100, 105, 31) };
            panel.Controls.AddRange(new Control[] { add, save, delete, refresh });
            info.Text = "Chọn loại phòng để sửa; sức chứa và mô tả chỉ thiết lập lúc thêm (giới hạn API hiện tại).";
            info.AutoSize = true; info.ForeColor = Color.DimGray; info.Location = new Point(18, 145); panel.Controls.Add(info);
            add.Click += (_, _) => ClearSelection();
            save.Click += async (_, _) => await SaveAsync();
            delete.Click += async (_, _) => await DeleteAsync();
            refresh.Click += async (_, _) => await LoadAsync();
            Load += async (_, _) => await LoadAsync();
        }

        private void SelectItem()
        {
            if (grid.CurrentRow?.DataBoundItem is not RoomTypeItem item) return;
            selected = item;
            nameBox.Text = item.TenLoai;
            priceBox.Value = Math.Min(priceBox.Maximum, Math.Max(priceBox.Minimum, item.GiaPhong));
            capacityBox.Value = Math.Min(capacityBox.Maximum, Math.Max(capacityBox.Minimum, item.SoNguoiToiDa));
            descriptionBox.Text = item.MoTa ?? "";
            capacityBox.Enabled = false;
            descriptionBox.ReadOnly = true;
        }

        private void ClearSelection()
        {
            selected = null; grid.ClearSelection(); nameBox.Clear(); priceBox.Value = 0;
            capacityBox.Enabled = true; capacityBox.Value = 2;
            descriptionBox.ReadOnly = false; descriptionBox.Clear(); nameBox.Focus();
        }

        private async Task LoadAsync()
        {
            try
            {
                using var response = await BaseApiClient.GetClient().GetAsync("LoaiPhong");
                if (!response.IsSuccessStatusCode) { await ShowError(response); return; }
                var list = await response.Content.ReadFromJsonAsync<List<RoomTypeItem>>() ?? new();
                grid.DataSource = null; grid.DataSource = list;
                ClearSelection();
            }
            catch (Exception ex) { MessageBox.Show("Không tải được loại phòng: " + ex.Message); }
        }

        private async Task SaveAsync()
        {
            if (busy) return;
            if (string.IsNullOrWhiteSpace(nameBox.Text) || priceBox.Value <= 0)
            { MessageBox.Show("Hãy nhập tên loại phòng và giá lớn hơn 0."); return; }
            busy = true;
            try
            {
                var body = new { TenLoai = nameBox.Text.Trim(), GiaPhong = priceBox.Value,
                    SoNguoiToiDa = (int)capacityBox.Value, MoTa = descriptionBox.Text.Trim() };
                using var response = selected == null
                    ? await BaseApiClient.GetClient().PostAsJsonAsync("LoaiPhong", body)
                    : await BaseApiClient.GetClient().PutAsJsonAsync($"LoaiPhong/{selected.LoaiPhongId}", body);
                if (!response.IsSuccessStatusCode) { await ShowError(response); return; }
                MessageBox.Show("Đã lưu loại phòng. Các phòng cùng loại sẽ sử dụng giá mới.", "Thành công");
                await LoadAsync();
            }
            catch (Exception ex) { MessageBox.Show("Không thể lưu loại phòng: " + ex.Message); }
            finally { busy = false; }
        }

        private async Task DeleteAsync()
        {
            if (busy || selected == null) { MessageBox.Show("Hãy chọn loại phòng cần xóa."); return; }
            if (MessageBox.Show($"Xóa loại phòng '{selected.TenLoai}'? Chỉ xóa được khi không có phòng liên kết.",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            busy = true;
            try
            {
                using var response = await BaseApiClient.GetClient().DeleteAsync($"LoaiPhong/{selected.LoaiPhongId}");
                if (!response.IsSuccessStatusCode) { await ShowError(response); return; }
                await LoadAsync();
            }
            catch (Exception ex) { MessageBox.Show("Không thể xóa loại phòng: " + ex.Message); }
            finally { busy = false; }
        }

        private static async Task ShowError(System.Net.Http.HttpResponseMessage response)
        {
            MessageBox.Show($"API trả HTTP {(int)response.StatusCode}:\n{await response.Content.ReadAsStringAsync()}",
                "Không thể thực hiện", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
