using HotelManagement.Desktop.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HotelManager.UI
{
    public partial class FrmManageUsers : Form
    {
        private int _selectedUserId = 0;

        public FrmManageUsers()
        {
            InitializeComponent();
        }

        private async void FrmManageUsers_Load(object sender, EventArgs e)
        {
            InitRolesComboBox();
            SetupDataGridView();
            await LoadUsersAsync();
        }

        private void InitRolesComboBox()
        {
            cboRole.Items.Clear();
            cboRole.Items.Add("Staff");
            cboRole.Items.Add("Admin");
            cboRole.SelectedIndex = 0;
        }

        private void SetupDataGridView()
        {
            dgvUsers.AutoGenerateColumns = true;
            dgvUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUsers.MultiSelect = false;
            dgvUsers.ReadOnly = true;
        }

        // 1. TẢI DANH SÁCH TÀI KHOẢN TỪ API (GET)
        private async Task LoadUsersAsync()
        {
            try
            {
                using (HttpClient client = BaseApiClient.GetClient())
                {
                    HttpResponseMessage response = await client.GetAsync("api/TaiKhoan");
                    if (response.IsSuccessStatusCode)
                    {
                        var users = await response.Content.ReadFromJsonAsync<List<UserAccountViewModel>>();
                        dgvUsers.DataSource = users ?? new List<UserAccountViewModel>();
                        ClearInputFields();
                    }
                    else
                    {
                        MessageBox.Show($"Không thể lấy danh sách tài khoản. Mã lỗi: {response.StatusCode}", "Lỗi API", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kết nối API: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 2. THÊM TÀI KHOẢN MỚI (POST)
        private async void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var userDto = new
            {
                Username = txtUsername.Text.Trim(),
                Password = txtPassword.Text.Trim(),
                FullName = txtFullName.Text.Trim(),
                Role = cboRole.SelectedItem?.ToString() ?? "Staff",
                IsActive = chkIsActive.Checked
            };

            try
            {
                using (HttpClient client = BaseApiClient.GetClient())
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync("api/TaiKhoan", userDto);
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Thêm tài khoản nhân viên mới thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        await LoadUsersAsync();
                    }
                    else
                    {
                        MessageBox.Show($"Thêm tài khoản thất bại. Mã lỗi API: {response.StatusCode}", "Lỗi API", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kết nối khi thêm tài khoản: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 3. CẬP NHẬT TÀI KHOẢN (PUT)
        private async void btnEdit_Click(object sender, EventArgs e)
        {
            if (_selectedUserId <= 0)
            {
                MessageBox.Show("Vui lòng chọn tài khoản cần cập nhật từ danh sách!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var userDto = new
            {
                Id = _selectedUserId,
                Username = txtUsername.Text.Trim(),
                Password = txtPassword.Text.Trim(), // Có thể để trống nếu không đổi password
                FullName = txtFullName.Text.Trim(),
                Role = cboRole.SelectedItem?.ToString() ?? "Staff",
                IsActive = chkIsActive.Checked
            };

            try
            {
                using (HttpClient client = BaseApiClient.GetClient())
                {
                    HttpResponseMessage response = await client.PutAsJsonAsync($"api/TaiKhoan/{_selectedUserId}", userDto);
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Cập nhật thông tin tài khoản thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        await LoadUsersAsync();
                    }
                    else
                    {
                        MessageBox.Show($"Cập nhật thất bại. Mã lỗi API: {response.StatusCode}", "Lỗi API", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kết nối khi cập nhật: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 4. XÓA TÀI KHOẢN (DELETE)
        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedUserId <= 0)
            {
                MessageBox.Show("Vui lòng chọn tài khoản cần xóa từ danh sách!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show($"Xác nhận XÓA tài khoản '{txtUsername.Text}'?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                using (HttpClient client = BaseApiClient.GetClient())
                {
                    HttpResponseMessage response = await client.DeleteAsync($"api/TaiKhoan/{_selectedUserId}");
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Xóa tài khoản thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        await LoadUsersAsync();
                    }
                    else
                    {
                        MessageBox.Show($"Xóa thất bại. Mã lỗi API: {response.StatusCode}", "Lỗi API", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kết nối khi xóa: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // HELPER & SỰ KIỆN GIAO DIỆN
        private void dgvUsers_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count > 0)
            {
                var user = dgvUsers.SelectedRows[0].DataBoundItem as UserAccountViewModel;
                if (user != null)
                {
                    _selectedUserId = user.Id;
                    txtUsername.Text = user.Username;
                    txtPassword.Text = user.Password;
                    txtFullName.Text = user.FullName;
                    cboRole.SelectedItem = user.Role;
                    chkIsActive.Checked = user.IsActive;
                }
            }
        }

        private void ClearInputFields()
        {
            _selectedUserId = 0;
            txtUsername.Clear();
            txtPassword.Clear();
            txtFullName.Clear();
            if (cboRole.Items.Count > 0) cboRole.SelectedIndex = 0;
            chkIsActive.Checked = true;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearInputFields();
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await LoadUsersAsync();
        }
    }
}