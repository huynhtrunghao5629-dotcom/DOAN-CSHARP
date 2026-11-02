using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using HotelManagement.Desktop.Models;
using HotelManager.UI;
using HotelManager;
using HotelManager.Services;

namespace HotelManager.UI
{
    public partial class FrmManageCustomers : Form
    {
        private int _selectedCustomerId = 0;

        public FrmManageCustomers()
        {
            InitializeComponent();
        }

        // 1. TẢI DỮ LIỆU KHI MỞ FORM
        private async void FrmManageCustomers_Load(object sender, EventArgs e)
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var client = BaseApiClient.GetClient();
                var response = await client.GetAsync("KhachHang");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var customers = JsonSerializer.Deserialize<List<CustomerDto>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    dgvCustomers.DataSource = null;
                    dgvCustomers.AutoGenerateColumns = true;
                    dgvCustomers.DataSource = customers;
                }
                else
                {
                    MessageBox.Show("Lỗi tải dữ liệu: " + response.StatusCode, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 2. CLICK VÀO BẢNG ĐỂ HIỂN THỊ LÊN TEXTBOX
        private void dgvCustomers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Đã bổ sung: Chặn lỗi văng phần mềm khi click bậy vào tiêu đề cột
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            DataGridViewRow row = dgvCustomers.Rows[e.RowIndex];

            // Lấy ID theo thuộc tính KhachHangId hoặc CustomerId
            if (row.Cells["KhachHangId"].Value != null)
                _selectedCustomerId = Convert.ToInt32(row.Cells["KhachHangId"].Value);
            else if (row.Cells["CustomerId"].Value != null)
                _selectedCustomerId = Convert.ToInt32(row.Cells["CustomerId"].Value);

            txtFullName.Text = row.Cells["HoTen"].Value?.ToString() ?? row.Cells["FullName"].Value?.ToString();
            txtPhone.Text = row.Cells["SoDienThoai"].Value?.ToString() ?? row.Cells["PhoneNumber"].Value?.ToString();
            txtEmail.Text = row.Cells["Email"].Value?.ToString();
            txtIdentityCard.Text = row.Cells["Cccd"].Value?.ToString() ?? row.Cells["IdentityCard"].Value?.ToString();
        }

        // 3. NÚT LƯU (THÊM / SỬA)
        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Họ tên không được để trống!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var customerData = new
            {
                KhachHangId = _selectedCustomerId,
                HoTen = txtFullName.Text.Trim(),
                SoDienThoai = txtPhone.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Cccd = txtIdentityCard.Text.Trim()
            };

            try
            {
                var client = BaseApiClient.GetClient();
                var content = new StringContent(JsonSerializer.Serialize(customerData), Encoding.UTF8, "application/json");

                HttpResponseMessage response;
                if (_selectedCustomerId == 0)
                {
                    response = await client.PostAsync("KhachHang", content);
                }
                else
                {
                    response = await client.PutAsync($"KhachHang/{_selectedCustomerId}", content);
                }

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Lưu thông tin khách hàng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ResetForm();
                    await LoadDataAsync();
                }
                else
                {
                    var errorMsg = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Lỗi từ server ({response.StatusCode}): {errorMsg}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối API: " + ex.Message, "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 4. NÚT XÓA KHÁCH HÀNG
        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedCustomerId == 0)
            {
                MessageBox.Show("Vui lòng chọn khách hàng cần xóa từ bảng!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show("Bạn có chắc chắn muốn xóa khách hàng này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                try
                {
                    var client = BaseApiClient.GetClient();
                    var response = await client.DeleteAsync($"KhachHang/{_selectedCustomerId}");
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ResetForm();
                        await LoadDataAsync();
                    }
                    else
                    {
                        var errorMsg = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Chức năng xóa chưa hỗ trợ hoặc bị lỗi: {errorMsg}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi kết nối API: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // 5. NÚT RESET FORM
        private void btnAdd_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        private void ResetForm()
        {
            _selectedCustomerId = 0;
            txtFullName.Clear();
            txtPhone.Clear();
            txtEmail.Clear();
            txtIdentityCard.Clear();
            txtFullName.Focus();
        }

        // 6. CHỨC NĂNG TÌM KIẾM (Đã bổ sung hoàn chỉnh)
        private async void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            try
            {
                var client = BaseApiClient.GetClient();
                HttpResponseMessage response;

                // Nếu ô tìm kiếm trống -> tải lại toàn bộ. Nếu có chữ -> gọi API tìm kiếm.
                if (string.IsNullOrEmpty(keyword))
                {
                    response = await client.GetAsync("KhachHang");
                }
                else
                {
                    response = await client.GetAsync($"KhachHang/tim-kiem?tuKhoa={keyword}");
                }

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var customers = JsonSerializer.Deserialize<List<CustomerDto>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    dgvCustomers.DataSource = null;
                    dgvCustomers.AutoGenerateColumns = true;
                    dgvCustomers.DataSource = customers;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi tìm kiếm: " + ex.Message);
            }
        }

        // Các hàm rỗng tránh lỗi Designer
        private void dgvCustomers_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void dgvCustomers_DataError(object sender, DataGridViewDataErrorEventArgs e) { }
    }
}