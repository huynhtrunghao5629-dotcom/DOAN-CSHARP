using HotelManager.Services;
using HotelManagement.Desktop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HotelManager.UI
{
    public partial class FrmManageBookings : Form
    {
        private List<BookingViewModel> _allBookings = new();
        private bool _busy;

        public FrmManageBookings()
        {
            InitializeComponent();
        }

        private async void FrmManageBookings_Load(object sender, EventArgs e)
        {
            cboStatusFilter.Items.Clear();
            cboStatusFilter.Items.AddRange(new object[] { "Tất cả", "Chờ xác nhận", "Đã đặt / Đã xác nhận", "Đang ở", "Đã trả phòng", "Đã hủy" });
            cboStatusFilter.SelectedIndex = 0;
            dgvBookings.AutoGenerateColumns = false;
            dgvBookings.Columns.Clear();
            AddColumn("Id", "Mã đơn", 65);
            AddColumn("TenKhachHang", "Khách hàng", 160);
            AddColumn("SoPhong", "Phòng", 90);
            AddColumn("NgayNhan", "Ngày nhận", 120, "dd/MM/yyyy");
            AddColumn("NgayTra", "Ngày trả", 120, "dd/MM/yyyy");
            AddColumn("TongTien", "Tổng tiền", 110, "N0");
            AddColumn("TrangThai", "Trạng thái", 130);
            dgvBookings.CellFormatting += (_, args) =>
            {
                if (args.RowIndex >= 0 && args.ColumnIndex >= 0 &&
                    dgvBookings.Columns[args.ColumnIndex].DataPropertyName == "TrangThai" &&
                    args.Value is string status)
                {
                    args.Value = TrangThaiTiengViet.Don(status);
                    args.FormattingApplied = true;
                }
            };
            dgvBookings.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvBookings.MultiSelect = false;
            dgvBookings.ReadOnly = true;

            // Hai quy trình này phải chờ API có hợp đồng thống nhất, tránh ghi sai dữ liệu.
            btnCreateAtCounter.Enabled = false;
            btnCreateAtCounter.Text = "Đặt tại quầy (*)";
            btnCheckOut.Enabled = false;
            btnCheckOut.Text = "Trả phòng (*)";
            await LoadBookingsAsync();
        }

        private void AddColumn(string property, string header, int width, string? format = null)
        {
            var col = new DataGridViewTextBoxColumn
            {
                Name = property,
                DataPropertyName = property,
                HeaderText = header,
                Width = width,
                ReadOnly = true
            };
            if (format != null) col.DefaultCellStyle.Format = format;
            dgvBookings.Columns.Add(col);
        }

        private static string CanonicalStatus(string? status) => (status ?? "").Trim().ToLowerInvariant() switch
        {
            "choxacnhan" or "chờ xác nhận" => "pending",
            "dadat" or "daxacnhan" => "confirmed",
            "dango" or "danhanphong" => "occupied",
            "datra" or "datraphong" or "dathanhtoan" => "completed",
            "dahuy" or "đã hủy" => "cancelled",
            _ => "unknown"
        };

        private async Task LoadBookingsAsync()
        {
            try
            {
                _busy = true;
                UpdateControlStates();
                btnRefresh.Enabled = false;
                using var response = await BaseApiClient.GetClient().GetAsync("DatPhong");
                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Không tải được đơn đặt phòng: {(int)response.StatusCode} {response.ReasonPhrase}", "Lỗi API");
                    return;
                }
                _allBookings = await response.Content.ReadFromJsonAsync<List<BookingViewModel>>() ?? new();
                ApplyFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không kết nối được API: {ex.Message}", "Lỗi kết nối");
            }
            finally
            {
                _busy = false;
                btnRefresh.Enabled = true;
                UpdateControlStates();
            }
        }

        private void ApplyFilter()
        {
            var selection = cboStatusFilter.SelectedItem?.ToString() ?? "Tất cả";
            string? key = selection switch
            {
                "Chờ xác nhận" => "pending",
                "Đã đặt / Đã xác nhận" => "confirmed",
                "Đang ở" => "occupied",
                "Đã trả phòng" => "completed",
                "Đã hủy" => "cancelled",
                _ => null
            };
            dgvBookings.DataSource = (key == null ? _allBookings : _allBookings.Where(b => CanonicalStatus(b.TrangThai) == key)).ToList();
            UpdateControlStates();
        }

        private BookingViewModel? SelectedBooking => dgvBookings.SelectedRows.Count > 0
            ? dgvBookings.SelectedRows[0].DataBoundItem as BookingViewModel
            : null;

        private void UpdateControlStates()
        {
            var status = CanonicalStatus(SelectedBooking?.TrangThai);
            btnConfirm.Enabled = !_busy && status == "pending";
            btnCheckIn.Enabled = !_busy && status == "confirmed";
            btnCancel.Enabled = !_busy && (status == "pending" || status == "confirmed");
            // Check-out cần lưu hóa đơn và thay đổi trạng thái trong một giao dịch ở API.
            btnCheckOut.Enabled = false;
        }

        private async Task ChangeStatusAsync(int bookingId, string status, string message)
        {
            try
            {
                _busy = true;
                UpdateControlStates();
                using var response = await BaseApiClient.GetClient()
                    .PutAsJsonAsync($"DatPhong/{bookingId}/trang-thai", new { TrangThai = status });
                if (!response.IsSuccessStatusCode)
                {
                    var detail = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Không cập nhật được đơn. HTTP {(int)response.StatusCode}: {detail}", "Lỗi API");
                    return;
                }
                MessageBox.Show(message, "Thành công");
                await LoadBookingsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không cập nhật được đơn: {ex.Message}", "Lỗi kết nối");
            }
            finally
            {
                _busy = false;
                UpdateControlStates();
            }
        }

        private async void btnConfirm_Click(object sender, EventArgs e)
        {
            var booking = SelectedBooking;
            if (booking == null || CanonicalStatus(booking.TrangThai) != "pending") return;
            if (MessageBox.Show($"Xác nhận đơn #{booking.Id}?", "Duyệt đơn", MessageBoxButtons.YesNo) == DialogResult.Yes)
                await ChangeStatusAsync(booking.Id, "DaDat", "Đã xác nhận đơn.");
        }

        private async void btnCheckIn_Click(object sender, EventArgs e)
        {
            var booking = SelectedBooking;
            if (booking == null || CanonicalStatus(booking.TrangThai) != "confirmed") return;
            if (MessageBox.Show($"Cho khách nhận phòng theo đơn #{booking.Id}?", "Nhận phòng", MessageBoxButtons.YesNo) == DialogResult.Yes)
                await ChangeStatusAsync(booking.Id, "DaNhanPhong", "Đã nhận phòng.");
        }

        private async void btnCancel_Click(object sender, EventArgs e)
        {
            var booking = SelectedBooking;
            if (booking == null || !(new[] { "pending", "confirmed" }).Contains(CanonicalStatus(booking.TrangThai))) return;
            if (MessageBox.Show($"Hủy đơn #{booking.Id}?", "Hủy đơn", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                await ChangeStatusAsync(booking.Id, "DaHuy", "Đã hủy đơn.");
        }

        private void btnCreateAtCounter_Click(object sender, EventArgs e) =>
            MessageBox.Show("Chức năng đặt tại quầy cần API chuẩn hóa KhachHangId, PhongId và kiểm tra lịch trùng.");

        private void btnCheckOut_Click(object sender, EventArgs e) =>
            MessageBox.Show("Check-out đang tạm khóa: phải hoàn thiện lưu thanh toán và hóa đơn phía API.");

        private async void btnRefresh_Click(object sender, EventArgs e) => await LoadBookingsAsync();
        private void cboStatusFilter_SelectedIndexChanged(object sender, EventArgs e) => ApplyFilter();
        private void dgvBookings_SelectionChanged(object sender, EventArgs e) => UpdateControlStates();
    }
}
