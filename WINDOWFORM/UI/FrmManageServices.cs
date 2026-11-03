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
    public partial class FrmManageServices : Form
    {
        private List<ChiTietDichVuViewModel> _currentServices = new();
        private readonly Dictionary<int, int> _bookingsByRoomId = new();
        private bool _loadingRooms;

        public FrmManageServices()
        {
            InitializeComponent();
        }

        private async void FrmManageServices_Load(object sender, EventArgs e)
        {
            dgvRoomServices.AutoGenerateColumns = true;
            dgvRoomServices.DataBindingComplete += (sender, args) => TieuDeTiengViet.GanCho(dgvRoomServices);
            dgvRoomServices.BackgroundColor = System.Drawing.Color.White;
            dgvRoomServices.ReadOnly = true;
            dgvRoomServices.MultiSelect = false;
            dgvRoomServices.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Đã bổ sung GET /api/DichVu để đọc danh mục thực.
            // Chưa cho ghi/xóa dịch vụ khi chưa đồng bộ đủ API và phân quyền.
            btnAddService.Enabled = false;
            btnDeleteService.Enabled = false;
            cboServices.Enabled = false;
            nudQuantity.Enabled = false;
            grpInput.Text = "Dịch vụ phát sinh — xem dữ liệu thật (chưa hỗ trợ thêm/xóa)";
            cboServices.DataSource = new List<DichVuViewModel>();
            txtPrice.Clear();
            await LoadDanhMucDichVuAsync();
            await LoadOccupiedRoomsAsync();
        }

        private async Task LoadDanhMucDichVuAsync()
        {
            try
            {
                using var response = await BaseApiClient.GetClient().GetAsync("DichVu");
                if (!response.IsSuccessStatusCode)
                    throw new InvalidOperationException($"HTTP {(int)response.StatusCode}");
                var ds = await response.Content.ReadFromJsonAsync<List<DichVuViewModel>>()
                    ?? new List<DichVuViewModel>();
                cboServices.DisplayMember = "TenDichVu";
                cboServices.ValueMember = "Id";
                cboServices.DataSource = ds;
                // Chỉ đọc; thao tác thêm/xóa đòi hợp đồng API và phân quyền hoàn chỉnh.
                cboServices.Enabled = false;
                if (ds.Count == 0) grpInput.Text = "Chưa có dịch vụ trong cơ sở dữ liệu";
            }
            catch (Exception ex)
            {
                grpInput.Text = "Không tải được danh mục dịch vụ";
                MessageBox.Show($"Không lấy được danh mục dịch vụ: {ex.Message}",
                    "Kết nối API", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async Task LoadOccupiedRoomsAsync()
        {
            try
            {
                btnRefresh.Enabled = false;
                _loadingRooms = true;
                var client = BaseApiClient.GetClient();
                using var response = await client.GetAsync("DatPhong");
                if (!response.IsSuccessStatusCode)
                    throw new InvalidOperationException($"API DatPhong trả về {(int)response.StatusCode} ({response.ReasonPhrase}).");

                var bookings = await response.Content.ReadFromJsonAsync<List<BookingViewModel>>()
                    ?? new List<BookingViewModel>();
                var active = bookings.Where(b => b.TrangThai == "DangO" || b.TrangThai == "DaNhanPhong").ToList();

                _bookingsByRoomId.Clear();
                var rooms = new List<PhongViewModel>();
                foreach (var booking in active)
                {
                    foreach (var room in booking.DanhSachPhong ?? new List<BookingRoomItem>())
                    {
                        if (_bookingsByRoomId.ContainsKey(room.PhongId)) continue;
                        _bookingsByRoomId[room.PhongId] = booking.Id;
                        rooms.Add(new PhongViewModel
                        {
                            PhongId = room.PhongId,
                            SoPhong = room.SoPhong,
                            TrangThai = "DangO",
                            GiaPhong = room.GiaPhong
                        });
                    }
                }
                cboOccupiedRooms.DataSource = null;
                cboOccupiedRooms.DisplayMember = "SoPhong";
                cboOccupiedRooms.ValueMember = "PhongId";
                cboOccupiedRooms.DataSource = rooms;
                if (rooms.Count == 0)
                    grpInput.Text = "Chưa có đơn đang nhận phòng (trạng thái Đang ở)";
                _currentServices.Clear();
                dgvRoomServices.DataSource = null;
                CalculateTotalAmount();
            }
            catch (Exception ex)
            {
                _bookingsByRoomId.Clear();
                cboOccupiedRooms.DataSource = null;
                _currentServices.Clear();
                dgvRoomServices.DataSource = null;
                CalculateTotalAmount();
                MessageBox.Show($"Không tải được danh sách đơn đang ở: {ex.Message}", "Lỗi API", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _loadingRooms = false;
                btnRefresh.Enabled = true;
            }

            if (cboOccupiedRooms.SelectedItem is PhongViewModel roomSelected)
                await LoadRoomServicesAsync(roomSelected.PhongId);
        }

        private async Task LoadRoomServicesAsync(int roomId)
        {
            _currentServices.Clear();
            dgvRoomServices.DataSource = null;
            CalculateTotalAmount();
            if (!_bookingsByRoomId.TryGetValue(roomId, out int bookingId)) return;

            try
            {
                var client = BaseApiClient.GetClient();
                using var response = await client.GetAsync($"SuDungDichVu/dat-phong/{bookingId}");
                if (!response.IsSuccessStatusCode)
                    throw new InvalidOperationException($"API trả về {(int)response.StatusCode} ({response.ReasonPhrase}).");

                var items = await response.Content.ReadFromJsonAsync<List<ChiTietDichVuViewModel>>()
                    ?? new List<ChiTietDichVuViewModel>();
                // Không để dữ liệu của phòng vừa tải ghi đè nếu người dùng đã đổi phòng.
                if (cboOccupiedRooms.SelectedItem is not PhongViewModel selected || selected.PhongId != roomId) return;
                _currentServices = items;
                dgvRoomServices.DataSource = _currentServices;
                CalculateTotalAmount();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không tải được dịch vụ của đơn #{bookingId}: {ex.Message}", "Lỗi API", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnAddService_Click(object sender, EventArgs e) => ShowPending();
        private void btnDeleteService_Click(object sender, EventArgs e) => ShowPending();

        private static void ShowPending() => MessageBox.Show(
            "API hiện chưa có danh mục DichVu và thao tác dịch vụ cần gắn với DatPhongId. " +
            "Vui lòng hoàn thiện API và phân quyền trước khi cho phép thêm/xóa.",
            "Chưa tích hợp", MessageBoxButtons.OK, MessageBoxIcon.Information);

        private async void cboOccupiedRooms_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loadingRooms) return;
            if (cboOccupiedRooms.SelectedItem is PhongViewModel room)
                await LoadRoomServicesAsync(room.PhongId);
        }

        private void cboServices_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtPrice.Text = cboServices.SelectedItem is DichVuViewModel service
                ? service.DonGia.ToString("N0") : string.Empty;
        }

        private void CalculateTotalAmount() =>
            lblTotalAmount.Text = $"{_currentServices.Sum(s => s.ThanhTien):N0} VNĐ";

        private async void btnRefresh_Click(object sender, EventArgs e) => await LoadOccupiedRoomsAsync();
    }
}
