using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using HotelManager.Services;
using HotelManagement.Desktop.Models;

namespace HotelManager.UI
{
    public partial class FrmManageServices : Form
    {
        private int currentBookingId;
        private int currentServiceRequest;
        private bool busy;

        public FrmManageServices()
        {
            InitializeComponent();
            Text = "Dịch vụ phát sinh của khách đang lưu trú";
        }

        private async void FrmManageServices_Load(object sender, EventArgs e)
        {
            cboOccupiedRooms.DropDownStyle = ComboBoxStyle.DropDownList;
            cboServices.DropDownStyle = ComboBoxStyle.DropDownList;
            txtPrice.ReadOnly = true;
            nudQuantity.Minimum = 1;
            nudQuantity.Maximum = 1000;
            dgvRoomServices.AutoGenerateColumns = false;
            dgvRoomServices.Columns.Clear();
            dgvRoomServices.AllowUserToAddRows = false;
            dgvRoomServices.ReadOnly = true;
            dgvRoomServices.MultiSelect = false;
            dgvRoomServices.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRoomServices.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvRoomServices.BackgroundColor = Color.White;
            dgvRoomServices.RowHeadersVisible = false;
            AddColumn("TenDichVu", "Dịch vụ", 150);
            AddColumn("SoLuong", "Số lượng", 65);
            AddColumn("DonGia", "Đơn giá", 95, "N0");
            AddColumn("ThanhTien", "Thành tiền", 110, "N0");
            cboServices.SelectedIndexChanged += (_, _) => UpdatePrice();
            // Không thay đổi Top/Height/Anchor thủ công: bố cục quản lý bằng 3 hàng.
            var update = new Button
            {
                Name = "btnUpdateQuantity",
                Text = "Cập nhật số lượng đã chọn",
                Size = new Size(215, 31),
                Location = new Point(620, 112)
            };
            grpInput.Controls.Add(update);
            update.Click += async (_, _) => await ChangeQuantityAsync();
            BuildServiceLayout();
            dgvRoomServices.SelectionChanged += (_, _) =>
            {
                if (dgvRoomServices.CurrentRow?.DataBoundItem is ChiTietDichVuViewModel item)
                    nudQuantity.Value = Math.Min(nudQuantity.Maximum, Math.Max(nudQuantity.Minimum, item.SoLuong));
            };
            await LoadServicesAsync();
            await LoadBookingsAsync();
        }

        private void BuildServiceLayout()
        {
            SuspendLayout();
            MinimumSize = new Size(900, 540);
            ClientSize = new Size(970, 640);

            Controls.Remove(grpInput);
            Controls.Remove(dgvRoomServices);
            Controls.Remove(lblTotalText);
            Controls.Remove(lblTotalAmount);

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(12),
                BackColor = Color.FromArgb(245, 247, 250)
            };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 186));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));

            grpInput.Dock = DockStyle.Fill;
            grpInput.Margin = new Padding(0, 0, 0, 10);
            grpInput.Anchor = AnchorStyles.None;

            dgvRoomServices.Dock = DockStyle.Fill;
            dgvRoomServices.Anchor = AnchorStyles.None;
            dgvRoomServices.Margin = new Padding(0);

            var footer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(12, 8, 12, 8),
                Margin = new Padding(0, 8, 0, 0)
            };
            // Dùng FlowLayoutPanel để hai nhãn không bị cắt khi đổi độ phân giải.
            var totals = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                AutoSize = false,
                Width = 420,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = Color.Transparent
            };
            lblTotalText.AutoSize = true;
            lblTotalText.Anchor = AnchorStyles.None;
            lblTotalText.Margin = new Padding(0, 5, 12, 0);
            lblTotalAmount.AutoSize = true;
            lblTotalAmount.Anchor = AnchorStyles.None;
            lblTotalAmount.Margin = new Padding(0, 3, 0, 0);
            totals.Controls.Add(lblTotalText);
            totals.Controls.Add(lblTotalAmount);
            footer.Controls.Add(totals);

            root.Controls.Add(grpInput, 0, 0);
            root.Controls.Add(dgvRoomServices, 0, 1);
            root.Controls.Add(footer, 0, 2);
            Controls.Add(root);
            root.BringToFront();
            ResumeLayout(true);
        }

        private static string TenDichVuTiengViet(string? name)
        {
            var text = (name ?? "").Trim();
            // Chuẩn hóa các tên đang được lưu không dấu trong dữ liệu mẫu.
            return text.ToLowerInvariant() switch
            {
                "an sang" => "Ăn sáng",
                "nuoc suoi" => "Nước suối",
                "thue xe" => "Thuê xe",
                "giat ui" => "Giặt ủi",
                _ => text
            };
        }

        private void AddColumn(string property, string title, int weight, string? format = null)
        {
            var column = new DataGridViewTextBoxColumn { DataPropertyName = property, HeaderText = title, FillWeight = weight, ReadOnly = true };
            if (format != null) column.DefaultCellStyle.Format = format;
            dgvRoomServices.Columns.Add(column);
        }

        private async Task LoadServicesAsync()
        {
            try
            {
                var services = await BaseApiClient.GetClient().GetFromJsonAsync<List<DichVuViewModel>>("DichVu") ?? new();
                cboServices.DataSource = null;
                foreach (var service in services)
                    service.TenDichVu = TenDichVuTiengViet(service.TenDichVu);
                cboServices.DataSource = services;
                cboServices.DisplayMember = "TenDichVu";
                cboServices.ValueMember = "Id";
                UpdatePrice();
            }
            catch (Exception ex) { MessageBox.Show("Không tải được danh mục dịch vụ: " + ex.Message); }
        }

        private void UpdatePrice()
        {
            if (cboServices.SelectedItem is DichVuViewModel item)
                txtPrice.Text = item.DonGia.ToString("N0") + " VNĐ";
            else txtPrice.Clear();
        }

        private async Task LoadBookingsAsync()
        {
            try
            {
                var bookings = await BaseApiClient.GetClient().GetFromJsonAsync<List<BookingViewModel>>("DatPhong") ?? new();
                var occupied = bookings.Where(b => b.TrangThai == "DaNhanPhong").ToList();
                cboOccupiedRooms.DataSource = null;
                cboOccupiedRooms.DataSource = occupied;
                cboOccupiedRooms.DisplayMember = "DisplayBooking";
                cboOccupiedRooms.ValueMember = "Id";
                if (occupied.Count == 0)
                {
                    currentBookingId = 0;
                    dgvRoomServices.DataSource = null;
                    lblTotalAmount.Text = "0 VNĐ";
                }
            }
            catch (Exception ex) { MessageBox.Show("Không tải được đơn đang ở: " + ex.Message); }
        }

        private async void cboOccupiedRooms_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboOccupiedRooms.SelectedItem is BookingViewModel booking)
            {
                currentBookingId = booking.Id;
                await LoadUsedServicesAsync(booking.Id);
            }
        }

        private void cboServices_SelectedIndexChanged(object sender, EventArgs e) => UpdatePrice();

        private async Task LoadUsedServicesAsync(int bookingId)
        {
            var version = ++currentServiceRequest;
            try
            {
                var list = await BaseApiClient.GetClient()
                    .GetFromJsonAsync<List<ChiTietDichVuViewModel>>($"SuDungDichVu/dat-phong/{bookingId}") ?? new();
                if (version != currentServiceRequest || bookingId != currentBookingId) return;
                dgvRoomServices.DataSource = null;
                foreach (var service in list)
                    service.TenDichVu = TenDichVuTiengViet(service.TenDichVu);
                dgvRoomServices.DataSource = list;
                lblTotalAmount.Text = $"{list.Sum(x => x.ThanhTien):N0} VNĐ";
                dgvRoomServices.ClearSelection();
            }
            catch (Exception ex) { if (version == currentServiceRequest) MessageBox.Show("Không tải được dịch vụ đã dùng: " + ex.Message); }
        }

        private async void btnAddService_Click(object sender, EventArgs e)
        {
            if (busy) return;
            if (currentBookingId <= 0 || cboServices.SelectedItem is not DichVuViewModel service || nudQuantity.Value <= 0)
            { MessageBox.Show("Hãy chọn đơn đang ở, dịch vụ và số lượng."); return; }
            busy = true; btnAddService.Enabled = false;
            try
            {
                using var res = await BaseApiClient.GetClient().PostAsJsonAsync("SuDungDichVu",
                    new { DatPhongId = currentBookingId, DichVuId = service.Id, SoLuong = (int)nudQuantity.Value });
                if (!res.IsSuccessStatusCode) { await ShowError(res); return; }
                await LoadUsedServicesAsync(currentBookingId);
            }
            catch (Exception ex) { MessageBox.Show("Không thêm được dịch vụ: " + ex.Message); }
            finally { busy = false; btnAddService.Enabled = true; }
        }

        private async Task ChangeQuantityAsync()
        {
            if (busy || dgvRoomServices.CurrentRow?.DataBoundItem is not ChiTietDichVuViewModel item) return;
            busy = true;
            try
            {
                using var res = await BaseApiClient.GetClient().PutAsJsonAsync($"SuDungDichVu/{item.Id}",
                    new { DatPhongId = currentBookingId, DichVuId = item.DichVuId, SoLuong = (int)nudQuantity.Value });
                if (!res.IsSuccessStatusCode) { await ShowError(res); return; }
                await LoadUsedServicesAsync(currentBookingId);
            }
            catch (Exception ex) { MessageBox.Show("Không cập nhật được số lượng: " + ex.Message); }
            finally { busy = false; }
        }

        private async void btnDeleteService_Click(object sender, EventArgs e)
        {
            if (busy || dgvRoomServices.CurrentRow?.DataBoundItem is not ChiTietDichVuViewModel item) return;
            if (MessageBox.Show($"Xóa dịch vụ '{item.TenDichVu}' khỏi đơn?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            busy = true;
            try
            {
                using var res = await BaseApiClient.GetClient().DeleteAsync($"SuDungDichVu/{item.Id}");
                if (!res.IsSuccessStatusCode) { await ShowError(res); return; }
                await LoadUsedServicesAsync(currentBookingId);
            }
            catch (Exception ex) { MessageBox.Show("Không xóa được dịch vụ: " + ex.Message); }
            finally { busy = false; }
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await LoadServicesAsync();
            await LoadBookingsAsync();
            if (currentBookingId > 0) await LoadUsedServicesAsync(currentBookingId);
        }

        private static async Task ShowError(System.Net.Http.HttpResponseMessage res)
        {
            // Không hiển thị stack trace dài của API cho người dùng cuối.
            var detail = await res.Content.ReadAsStringAsync();
            if (detail.Length > 350) detail = detail.Substring(0, 350) + "…";
            MessageBox.Show($"Yêu cầu dịch vụ thất bại (HTTP {(int)res.StatusCode}).\n{detail}",
                "Không thể cập nhật dịch vụ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
