using HotelManager.Services;
using HotelManagement.Desktop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HotelManager.UI
{
    public partial class FrmManageCustomers : Form
    {
        private int _selectedCustomerId;
        private int _searchVersion;
        private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

        public FrmManageCustomers()
        {
            InitializeComponent();
            dgvCustomers.AutoGenerateColumns = true;
            dgvCustomers.ReadOnly = true;
            dgvCustomers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCustomers.MultiSelect = false;
            dgvCustomers.AllowUserToAddRows = false;

            // Current API has NO DELETE /api/KhachHang/{id} route.
            // Do not show a misleading active button.
            btnDelete.Enabled = false;
            btnDelete.Text = "Xóa (chưa hỗ trợ)";
        }

        private async void FrmManageCustomers_Load(object sender, EventArgs e)
        {
            await LoadCustomersAsync();
        }

        private async Task LoadCustomersAsync(string? keyword = null, int? requestVersion = null)
        {
            try
            {
                var path = string.IsNullOrWhiteSpace(keyword)
                    ? "KhachHang"
                    : "KhachHang/tim-kiem?tuKhoa=" + Uri.EscapeDataString(keyword.Trim());
                using var response = await BaseApiClient.GetClient().GetAsync(path);
                if (!response.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException($"HTTP {(int)response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
                }
                var customers = await response.Content.ReadFromJsonAsync<List<CustomerDto>>(_jsonOptions)
                    ?? new List<CustomerDto>();

                // User may have typed another search while the old request was pending.
                if (requestVersion.HasValue && requestVersion.Value != _searchVersion) return;
                dgvCustomers.DataSource = null;
                dgvCustomers.DataSource = customers;
                TranslateHeaders();
            }
            catch (Exception ex)
            {
                if (requestVersion.HasValue && requestVersion.Value != _searchVersion) return;
                MessageBox.Show("Không thể tải khách hàng: " + ex.Message,
                    "Lỗi API", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void TranslateHeaders()
        {
            var names = new Dictionary<string, string>
            {
                ["CustomerId"] = "Mã khách hàng",
                ["FullName"] = "Họ và tên",
                ["IdentityCard"] = "CCCD/CMND",
                ["PhoneNumber"] = "Số điện thoại",
                ["Email"] = "Email",
                ["Address"] = "Địa chỉ"
            };
            foreach (DataGridViewColumn column in dgvCustomers.Columns)
                if (names.TryGetValue(column.DataPropertyName, out var header))
                    column.HeaderText = header;
        }

        private void dgvCustomers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvCustomers.Rows.Count) return;
            if (dgvCustomers.Rows[e.RowIndex].DataBoundItem is not CustomerDto customer) return;

            _selectedCustomerId = customer.CustomerId;
            txtFullName.Text = customer.FullName ?? "";
            txtPhone.Text = customer.PhoneNumber ?? "";
            txtEmail.Text = customer.Email ?? "";
            txtIdentityCard.Text = customer.IdentityCard ?? "";
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text) || string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Cần nhập họ tên và số điện thoại.", "Thiếu thông tin");
                return;
            }
            var payload = new
            {
                KhachHangId = _selectedCustomerId,
                HoTen = txtFullName.Text.Trim(),
                SoDienThoai = txtPhone.Text.Trim(),
                Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim(),
                Cccd = string.IsNullOrWhiteSpace(txtIdentityCard.Text) ? null : txtIdentityCard.Text.Trim()
            };
            try
            {
                btnSave.Enabled = false;
                var client = BaseApiClient.GetClient();
                using var response = _selectedCustomerId == 0
                    ? await client.PostAsJsonAsync("KhachHang", payload)
                    : await client.PutAsJsonAsync($"KhachHang/{_selectedCustomerId}", payload);
                if (!response.IsSuccessStatusCode)
                    throw new InvalidOperationException($"HTTP {(int)response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
                MessageBox.Show("Đã lưu khách hàng vào API.", "Thành công");
                ResetForm();
                await LoadCustomersAsync(txtSearch.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể lưu khách hàng: " + ex.Message, "Lỗi API");
            }
            finally { btnSave.Enabled = true; }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            MessageBox.Show("API hiện chưa hỗ trợ xóa khách hàng. Chức năng đang tắt để tránh hiểu nhầm.");
        }

        private void btnAdd_Click(object sender, EventArgs e) => ResetForm();

        private void ResetForm()
        {
            _selectedCustomerId = 0;
            txtFullName.Clear();
            txtPhone.Clear();
            txtEmail.Clear();
            txtIdentityCard.Clear();
            txtFullName.Focus();
        }

        private async void txtSearch_TextChanged(object sender, EventArgs e)
        {
            var requestVersion = ++_searchVersion;
            await LoadCustomersAsync(txtSearch.Text, requestVersion);
        }

        // Designer bindings in the original project.
        private void dgvCustomers_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void dgvCustomers_DataError(object sender, DataGridViewDataErrorEventArgs e) { }
    }
}
