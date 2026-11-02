using HotelManagement.Desktop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HotelManager.UI
{
    public partial class FrmManageServices : Form
    {
        private List<ChiTietDichVuViewModel> _currentServices = new List<ChiTietDichVuViewModel>();

        public FrmManageServices()
        {
            InitializeComponent();
        }

        private async void FrmManageServices_Load(object sender, EventArgs e)
        {
            SetupDataGridView();
            await LoadOccupiedRoomsAsync();
            await LoadAvailableServicesAsync();
        }

        private void SetupDataGridView()
        {
            dgvRoomServices.AutoGenerateColumns = true;
            dgvRoomServices.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRoomServices.MultiSelect = false;
            dgvRoomServices.ReadOnly = true;
        }

        // 1. TẢI DANH SÁCH PHÒNG ĐANG CÓ KHÁCH Ở (DangSuDung)
        private async Task LoadOccupiedRoomsAsync()
        {
            try
            {
                using (HttpClient client = BaseApiClient.GetClient())
                {
                    HttpResponseMessage response = await client.GetAsync("api/Phong");
                    if (response.IsSuccessStatusCode)
                    {
                        var allRooms = await response.Content.ReadFromJsonAsync<List<PhongViewModel>>();
                        var occupiedRooms = allRooms?.Where(r => r.TrangThai == "DangSuDung" || r.TrangThai == "DaNhanPhong").ToList() ?? new List<PhongViewModel>();

                        cboOccupiedRooms.DataSource = occupiedRooms;
                        cboOccupiedRooms.DisplayMember = "SoPhong";
                        cboOccupiedRooms.ValueMember = "PhongId";

                        if (occupiedRooms.Count == 0)
                        {
                            dgvRoomServices.DataSource = null;
                            CalculateTotalAmount();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách phòng: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 2. TẢI DANH SÁCH CÁC DỊCH VỤ SẴN CÓ CỦA KHÁCH SẠN
        private async Task LoadAvailableServicesAsync()
        {
            try
            {
                using (HttpClient client = BaseApiClient.GetClient())
                {
                    HttpResponseMessage response = await client.GetAsync("api/DichVu");
                    if (response.IsSuccessStatusCode)
                    {
                        var services = await response.Content.ReadFromJsonAsync<List<DichVuViewModel>>();
                        cboServices.DataSource = services ?? new List<DichVuViewModel>();
                        cboServices.DisplayMember = "TenDichVu";
                        cboServices.ValueMember = "Id";

                        UpdateSelectedServicePrice();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh mục dịch vụ: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 3. TẢI DANH SÁCH DỊCH VỤ ĐÃ DÙNG CỦA PHÒNG ĐƯỢC CHỌN
        private async Task LoadRoomServicesAsync(string phongId)
        {
            if (string.IsNullOrEmpty(phongId)) return;

            try
            {
                using (HttpClient client = BaseApiClient.GetClient())
                {
                    HttpResponseMessage response = await client.GetAsync($"api/ChiTietDichVu/phong/{phongId}");
                    if (response.IsSuccessStatusCode)
                    {
                        var services = await response.Content.ReadFromJsonAsync<List<ChiTietDichVuViewModel>>();
                        _currentServices = services ?? new List<ChiTietDichVuViewModel>();
                        dgvRoomServices.DataSource = _currentServices;
                        CalculateTotalAmount();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dịch vụ phòng: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 4. THÊM DỊCH VỤ PHÁT SINH VÀO PHÒNG (POST)
        private async void btnAddService_Click(object sender, EventArgs e)
        {
            if (cboOccupiedRooms.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn phòng đang có khách ở!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cboServices.SelectedItem is not DichVuViewModel selectedService)
            {
                MessageBox.Show("Vui lòng chọn dịch vụ cần thêm!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string phongId = cboOccupiedRooms.SelectedValue.ToString() ?? string.Empty;
            int quantity = (int)nudQuantity.Value;

            var payload = new
            {
                PhongId = phongId,
                DichVuId = selectedService.Id,
                SoLuong = quantity,
                DonGia = selectedService.DonGia,
                ThoiGian = DateTime.Now
            };

            try
            {
                using (HttpClient client = BaseApiClient.GetClient())
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync("api/ChiTietDichVu", payload);
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"Đã thêm dịch vụ '{selectedService.TenDichVu}' cho phòng thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        await LoadRoomServicesAsync(phongId);
                    }
                    else
                    {
                        MessageBox.Show($"Thêm dịch vụ thất bại. Mã lỗi API: {response.StatusCode}", "Lỗi API", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kết nối khi ghi nhận dịch vụ: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 5. XÓA MỘT DỊCH VỤ ĐÃ CHỌN TRÊN BẢNG (DELETE)
        private async void btnDeleteService_Click(object sender, EventArgs e)
        {
            if (dgvRoomServices.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một dòng dịch vụ cần xóa từ bảng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var item = dgvRoomServices.SelectedRows[0].DataBoundItem as ChiTietDichVuViewModel;
            if (item == null) return;

            var confirm = MessageBox.Show($"Xác nhận xóa dịch vụ '{item.TenDichVu}' ra khỏi phòng?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                using (HttpClient client = BaseApiClient.GetClient())
                {
                    HttpResponseMessage response = await client.DeleteAsync($"api/ChiTietDichVu/{item.Id}");
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Đã xóa dịch vụ thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        if (cboOccupiedRooms.SelectedValue != null)
                        {
                            await LoadRoomServicesAsync(cboOccupiedRooms.SelectedValue.ToString() ?? "");
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Xóa dịch vụ thất bại. Mã lỗi: {response.StatusCode}", "Lỗi API", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kết nối khi xóa: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 6. HELPER & SỰ KIỆN GIAO DIỆN
        private async void cboOccupiedRooms_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboOccupiedRooms.SelectedValue != null)
            {
                string phongId = cboOccupiedRooms.SelectedValue.ToString() ?? string.Empty;
                await LoadRoomServicesAsync(phongId);
            }
        }

        private void cboServices_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSelectedServicePrice();
        }

        private void UpdateSelectedServicePrice()
        {
            if (cboServices.SelectedItem is DichVuViewModel service)
            {
                txtPrice.Text = service.DonGia.ToString("N0");
            }
            else
            {
                txtPrice.Clear();
            }
        }

        private void CalculateTotalAmount()
        {
            decimal total = _currentServices.Sum(s => s.ThanhTien);
            lblTotalAmount.Text = $"{total:N0} VNĐ";
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await LoadOccupiedRoomsAsync();
            await LoadAvailableServicesAsync();
        }
    }
}