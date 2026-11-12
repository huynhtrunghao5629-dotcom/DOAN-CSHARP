using HotelManager.Services;
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
            cboRole.Items.Add("Nhân viên / Lễ tân");
            cboRole.Items.Add("Quản lý");
            cboRole.SelectedIndex = 0;
        }

        private void SetupDataGridView()
        {
            dgvUsers.AutoGenerateColumns = false;
            dgvUsers.Columns.Clear();
            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "Mã", FillWeight = 50 });
            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Username", HeaderText = "Tên đăng nhập", FillWeight = 145 });
            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Role", HeaderText = "Vai trò", FillWeight = 90 });
            dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvUsers.RowHeadersVisible = false;
            dgvUsers.AllowUserToAddRows = false;
            dgvUsers.BackgroundColor = System.Drawing.Color.White;
            dgvUsers.DataBindingComplete += (sender, args) => TieuDeTiengViet.GanCho(dgvUsers);
            dgvUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUsers.MultiSelect = false;
            dgvUsers.ReadOnly = true;
            txtFullName.Enabled = false; // API TaiKhoan không quản lý tên nhân viên.
            chkIsActive.Enabled = false; // CSDL không có trường IsActive.
            txtFullName.Visible = false;
            chkIsActive.Visible = false;
            txtPassword.UseSystemPasswordChar = true;
            foreach (Control c in grpInput.Controls)
                if (c is Label l && (l.Text.Contains("Họ tên") || l.Text.Contains("Hoạt động"))) l.Visible = false;
            dgvUsers.CellFormatting += (sender, args) =>
            {
                if (args.RowIndex < 0 || args.ColumnIndex < 0) return;
                string column = dgvUsers.Columns[args.ColumnIndex].DataPropertyName;
                if (column == "Role")
                {
                    args.Value = TrangThaiTiengViet.VaiTro(args.Value?.ToString());
                    args.FormattingApplied = true;
                }

            };
        }

        // 1. TẢI DANH SÁCH TÀI KHOẢN TỪ API (GET)
        private async Task LoadUsersAsync()
        {
            try
            {
                {
                HttpClient client = BaseApiClient.GetClient();
                    HttpResponseMessage response = await client.GetAsync("TaiKhoan");
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
                TenDangNhap = txtUsername.Text.Trim(),
                MatKhau = txtPassword.Text,
                VaiTro = cboRole.SelectedIndex == 1 ? "Admin" : "NhanVien"
            };

            try
            {
                {
                HttpClient client = BaseApiClient.GetClient();
                    HttpResponseMessage response = await client.PostAsJsonAsync("TaiKhoan", userDto);
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
                VaiTro = cboRole.SelectedIndex == 1 ? "Admin" : "NhanVien",
                MatKhau = string.IsNullOrWhiteSpace(txtPassword.Text) ? null : txtPassword.Text
            };

            try
            {
                {
                HttpClient client = BaseApiClient.GetClient();
                    HttpResponseMessage response = await client.PutAsJsonAsync($"TaiKhoan/{_selectedUserId}", userDto);
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
                {
                HttpClient client = BaseApiClient.GetClient();
                    HttpResponseMessage response = await client.DeleteAsync($"TaiKhoan/{_selectedUserId}");
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
                    txtPassword.Clear(); // API không trả mật khẩu; để trống = giữ nguyên mật khẩu.
                    txtFullName.Text = user.FullName;
                    cboRole.SelectedIndex = TrangThaiTiengViet.VaiTro(user.Role) == "Quản lý" ? 1 : 0;
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