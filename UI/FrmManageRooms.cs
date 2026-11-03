using HotelManagement.Desktop.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HotelManager.UI
{
    public partial class FrmManageRooms : Form
    {
        private string _selectedPhongId = string.Empty;

        public FrmManageRooms()
        {
            InitializeComponent();
        }

        private async void FrmManageRooms_Load(object sender, EventArgs e)
        {
            InitStatusComboBox();
            SetupDataGridView();
            await LoadRoomsAsync();
        }

        private void InitStatusComboBox()
        {
            cboTrangThai.Items.Clear();
            cboTrangThai.Items.Add("Trong");
            cboTrangThai.Items.Add("DangSuDung");
            cboTrangThai.Items.Add("DangBaoTri");
            cboTrangThai.SelectedIndex = 0;
        }

        private void SetupDataGridView()
        {
            dgvRooms.AutoGenerateColumns = true;
            dgvRooms.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRooms.MultiSelect = false;
            dgvRooms.ReadOnly = true;
        }

        private async Task LoadRoomsAsync()
        {
            try
            {
                using (HttpClient client = BaseApiClient.GetClient())
                {
                    HttpResponseMessage response = await client.GetAsync("api/Phong");
                    if (response.IsSuccessStatusCode)
                    {
                        var rooms = await response.Content.ReadFromJsonAsync<List<PhongViewModel>>();
                        dgvRooms.DataSource = rooms ?? new List<PhongViewModel>();
                        ClearInputFields();
                    }
                    else
                    {
                        MessageBox.Show($"Không thể lấy danh sách phòng. Mã lỗi: {response.StatusCode}", "Lỗi API", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kết nối API: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            var roomDto = new
            {
                PhongId = txtSoPhong.Text.Trim(), // Sử dụng SoPhong làm PhongId hoặc để API tự sinh
                SoPhong = txtSoPhong.Text.Trim(),
                TenLoaiPhong = txtTenLoaiPhong.Text.Trim(),
                GiaPhong = decimal.Parse(txtGiaPhong.Text.Trim()),
                TrangThai = cboTrangThai.SelectedItem?.ToString() ?? "Trong"
            };

            try
            {
                using (HttpClient client = BaseApiClient.GetClient())
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync("api/Phong", roomDto);
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Thêm phòng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        await LoadRoomsAsync();
                    }
                    else
                    {
                        MessageBox.Show($"Thêm phòng thất bại. Mã lỗi: {response.StatusCode}", "Lỗi API", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kết nối khi thêm phòng: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnEdit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedPhongId))
            {
                MessageBox.Show("Vui lòng chọn phòng cần cập nhật từ bảng danh sách!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateInput()) return;

            var roomDto = new
            {
                PhongId = _selectedPhongId,
                SoPhong = txtSoPhong.Text.Trim(),
                TenLoaiPhong = txtTenLoaiPhong.Text.Trim(),
                GiaPhong = decimal.Parse(txtGiaPhong.Text.Trim()),
                TrangThai = cboTrangThai.SelectedItem?.ToString() ?? "Trong"
            };

            try
            {
                using (HttpClient client = BaseApiClient.GetClient())
                {
                    HttpResponseMessage response = await client.PutAsJsonAsync($"api/Phong/{_selectedPhongId}", roomDto);
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Cập nhật phòng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        await LoadRoomsAsync();
                    }
                    else
                    {
                        MessageBox.Show($"Cập nhật thất bại. Mã lỗi: {response.StatusCode}", "Lỗi API", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kết nối khi cập nhật phòng: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedPhongId))
            {
                MessageBox.Show("Vui lòng chọn phòng cần xóa từ danh sách!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show($"Bạn có chắc chắn muốn XÓA phòng '{txtSoPhong.Text}' không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                using (HttpClient client = BaseApiClient.GetClient())
                {
                    HttpResponseMessage response = await client.DeleteAsync($"api/Phong/{_selectedPhongId}");
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Xóa phòng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        await LoadRoomsAsync();
                    }
                    else
                    {
                        MessageBox.Show($"Xóa thất bại. Mã lỗi API: {response.StatusCode}", "Lỗi API", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kết nối khi xóa phòng: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvRooms_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvRooms.SelectedRows.Count > 0)
            {
                var room = dgvRooms.SelectedRows[0].DataBoundItem as PhongViewModel;
                if (room != null)
                {
                    _selectedPhongId = room.PhongId;
                    txtSoPhong.Text = room.SoPhong;
                    txtTenLoaiPhong.Text = room.TenLoaiPhong;
                    txtGiaPhong.Text = room.GiaPhong.ToString("G0");
                    cboTrangThai.SelectedItem = room.TrangThai;
                }
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtSoPhong.Text))
            {
                MessageBox.Show("Vui lòng nhập số phòng!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSoPhong.Focus();
                return false;
            }

            if (!decimal.TryParse(txtGiaPhong.Text.Trim(), out decimal price) || price < 0)
            {
                MessageBox.Show("Giá phòng phải là một số hợp lệ lớn hơn hoặc bằng 0!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtGiaPhong.Focus();
                return false;
            }

            return true;
        }

        private void ClearInputFields()
        {
            _selectedPhongId = string.Empty;
            txtSoPhong.Clear();
            txtTenLoaiPhong.Clear();
            txtGiaPhong.Clear();
            if (cboTrangThai.Items.Count > 0) cboTrangThai.SelectedIndex = 0;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearInputFields();
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await LoadRoomsAsync();
        }
    }
}