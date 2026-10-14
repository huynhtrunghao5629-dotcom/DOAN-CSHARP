using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using HotelManagement.Desktop.Models;
using HotelManagement.Desktop.Services;

namespace HotelManagement.Desktop.UI
{
    public partial class FrmManageCustomers : Form
    {
        private readonly CustomerService _customerService;
        private int _selectedCustomerId = 0;
        private readonly UserSession _currentUser;

        public FrmManageCustomers()
        {
            InitializeComponent();
            _customerService = new CustomerService();
        }

        public FrmManageCustomers(UserSession userSession) : this()
        {
            _currentUser = userSession;
        }

        private async void FrmManageCustomers_Load(object sender, EventArgs e)
        {
            ApplyPermissions();
            await LoadCustomerDataAsync();
        }

        private void ApplyPermissions()
        {
            bool hasAccess = _currentUser != null &&
                             ("Admin".Equals(_currentUser.Role, StringComparison.OrdinalIgnoreCase) ||
                              "Receptionist".Equals(_currentUser.Role, StringComparison.OrdinalIgnoreCase));

            txtFullName.Enabled = hasAccess;
            txtPhone.Enabled = hasAccess;
            txtEmail.Enabled = hasAccess;
            txtIdentityCard.Enabled = hasAccess;
            btnAdd.Enabled = hasAccess;
            btnSave.Enabled = hasAccess;
            btnDelete.Enabled = hasAccess;
            txtSearch.Enabled = true;
        }

        private async Task LoadCustomerDataAsync()
        {
            try
            {
                var customers = await _customerService.GetAllCustomersAsync();
                dgvCustomers.DataSource = null;
                dgvCustomers.DataSource = customers;

                if (dgvCustomers.Columns["CustomerId"] != null)
                {
                    dgvCustomers.Columns["CustomerId"].ReadOnly = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvCustomers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow r = dgvCustomers.Rows[e.RowIndex];
                _selectedCustomerId = Convert.ToInt32(r.Cells["CustomerId"].Value);
                txtFullName.Text = r.Cells["FullName"].Value?.ToString();
                txtPhone.Text = r.Cells["PhoneNumber"].Value?.ToString();
                txtEmail.Text = r.Cells["Email"].Value?.ToString();
                txtIdentityCard.Text = r.Cells["IdentityCard"].Value?.ToString();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            _selectedCustomerId = 0;
            txtFullName.Clear();
            txtPhone.Clear();
            txtEmail.Clear();
            txtIdentityCard.Clear();
            txtFullName.Focus();
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtFullName.Text) || string.IsNullOrWhiteSpace(txtPhone.Text))
                {
                    MessageBox.Show("Vui lòng nhập tên và số điện thoại!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Khởi tạo đối tượng customer từ các TextBox trên giao diện
                var customer = new CustomerDto
                {
                    CustomerId = _selectedCustomerId,
                    FullName = txtFullName.Text.Trim(),
                    PhoneNumber = txtPhone.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    IdentityCard = txtIdentityCard.Text.Trim()
                };

                await _customerService.SaveCustomerAsync(customer);
                MessageBox.Show("Lưu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                btnAdd_Click(sender, e);
                await LoadCustomerDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedCustomerId == 0)
            {
                MessageBox.Show("Vui lòng chọn khách hàng cần xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Bạn có chắc chắn muốn xóa?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    await _customerService.DeleteCustomerAsync(_selectedCustomerId);
                    MessageBox.Show("Đã xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnAdd_Click(sender, e);
                    await LoadCustomerDataAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string keyword = txtSearch.Text.Trim().ToLower();
                var allCustomers = await _customerService.GetAllCustomersAsync();

                if (string.IsNullOrEmpty(keyword))
                {
                    dgvCustomers.DataSource = allCustomers;
                }
                else
                {
                    dgvCustomers.DataSource = allCustomers.Where(c =>
                        (c.FullName != null && c.FullName.ToLower().Contains(keyword)) ||
                        (c.PhoneNumber != null && c.PhoneNumber.Contains(keyword))
                    ).ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tìm kiếm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvCustomers_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }
    }
}