using HotelManager.Services;
using HotelManagement.Desktop.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.ComponentModel.DataAnnotations;
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
        private readonly System.Windows.Forms.Timer _searchTimer = new() { Interval = 350 };
        private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);
        private readonly TextBox txtAddress = new();

        public FrmManageCustomers()
        {
            InitializeComponent();
            dgvCustomers.AutoGenerateColumns = true;
            dgvCustomers.ReadOnly = true;
            dgvCustomers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCustomers.MultiSelect = false;
            dgvCustomers.AllowUserToAddRows = false;
            dgvCustomers.CellFormatting += (_, e) =>
            {
                if (dgvCustomers.Columns[e.ColumnIndex].DataPropertyName == "CustomerType" && e.Value != null)
                {
                    string loai = e.Value.ToString()!;
                    if (loai.Contains("Web"))
                    {
                        e.CellStyle!.ForeColor = Color.SeaGreen;
                        e.CellStyle.Font = new Font(dgvCustomers.Font, FontStyle.Bold);
                    }
                    else
                    {
                        e.CellStyle!.ForeColor = Color.DimGray;
                    }
                }
            };

            // Current API has NO DELETE /api/KhachHang/{id} route.
            // Do not show a misleading active button.
            btnDelete.Enabled = false;
            btnDelete.Text = "Xóa (chưa hỗ trợ)";
            BuildResponsiveLayout();
            _searchTimer.Tick += async (_, _) =>
            {
                _searchTimer.Stop();
                var version = _searchVersion;
                await LoadCustomersAsync(txtSearch.Text, version);
            };
        }

        // Form cũ Dock bảng ở sát đáy và để trống phần lớn cửa sổ.
        // Gom vùng nhập liệu vào phía trên, bảng luôn chiếm phần còn lại.
        private void BuildResponsiveLayout()
        {
            Text = "Quản lý khách hàng";
            MinimumSize = new Size(910, 580);
            ClientSize = new Size(1040, 640);
            Font = new Font("Segoe UI", 9.5F);

            // Tách hẳn biểu mẫu và bảng thành hai hàng.
            // Không dùng Dock.Fill cho DataGridView cùng cấp với Panel,
            // vì thứ tự Dock/Z-order dễ khiến bảng bị che hoặc biến mất.
            SuspendLayout();

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                BackColor = Color.White
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 196F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var editor = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = Padding.Empty,
                Padding = new Padding(16),
                BackColor = Color.FromArgb(245, 247, 250)
            };
            var fields = new Control[]
            {
                label1, label2, label3, label4, label5,
                txtFullName, txtPhone, txtEmail, txtIdentityCard,
                txtSearch, btnAdd, btnSave, btnDelete
            };
            foreach (var control in fields)
            {
                // Add tự chuyển control khỏi Form cũ sang editor.
                editor.Controls.Add(control);
            }

            label1.Location = new Point(18, 24);
            txtFullName.SetBounds(145, 20, 250, 28);
            label2.Location = new Point(18, 67);
            txtPhone.SetBounds(145, 63, 250, 28);
            label3.Location = new Point(18, 110);
            txtEmail.SetBounds(145, 106, 250, 28);
            label4.Location = new Point(435, 24);
            txtIdentityCard.SetBounds(555, 20, 235, 28);

            var lblAddress = new Label
            {
                Text = "Địa chỉ:",
                AutoSize = true,
                Location = new Point(435, 67)
            };
            editor.Controls.Add(lblAddress);
            txtAddress.SetBounds(555, 63, 235, 28);
            editor.Controls.Add(txtAddress);

            label5.Location = new Point(18, 153);
            txtSearch.SetBounds(145, 148, 250, 28);
            btnAdd.Text = "Thêm mới / Làm trống";
            btnAdd.SetBounds(435, 107, 165, 34);
            btnSave.Text = "Lưu khách hàng";
            btnSave.SetBounds(610, 107, 180, 34);
            btnDelete.SetBounds(805, 107, 180, 34);

            dgvCustomers.Dock = DockStyle.Fill;
            dgvCustomers.Margin = new Padding(12, 8, 12, 12);
            dgvCustomers.Visible = true;
            dgvCustomers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCustomers.RowHeadersVisible = false;
            dgvCustomers.AllowUserToResizeRows = false;
            dgvCustomers.BackgroundColor = Color.White;
            dgvCustomers.BorderStyle = BorderStyle.FixedSingle;
            dgvCustomers.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(246, 249, 252);
            dgvCustomers.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(34, 88, 111);
            dgvCustomers.ColumnHeadersHeight = 38;
            dgvCustomers.RowTemplate.Height = 31;

            layout.Controls.Add(editor, 0, 0);
            // Add tự chuyển DataGridView ra khỏi Form cũ vào đúng hàng 2.
            layout.Controls.Add(dgvCustomers, 0, 1);
            Controls.Add(layout);

            ResumeLayout(true);
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
                dgvCustomers.ClearSelection();
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
        ["CustomerType"] = "Phân loại",
        ["IdentityCard"] = "CCCD/CMND",
        ["PhoneNumber"] = "Số điện thoại",
        ["Email"] = "Email",
        ["Address"] = "Địa chỉ"
    };
    foreach (DataGridViewColumn column in dgvCustomers.Columns)
        if (names.TryGetValue(column.DataPropertyName, out var header))
            column.HeaderText = header;

    if (dgvCustomers.Columns["CustomerType"] != null)
        dgvCustomers.Columns["CustomerType"]!.DisplayIndex = 2;
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
            txtAddress.Text = customer.Address ?? "";
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text) || string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Cần nhập họ tên và số điện thoại.", "Thiếu thông tin");
                return;
            }
            if (!string.IsNullOrWhiteSpace(txtEmail.Text) &&
                !new EmailAddressAttribute().IsValid(txtEmail.Text.Trim()))
            {
                MessageBox.Show("Email không đúng định dạng.", "Kiểm tra dữ liệu");
                return;
            }
            if (_selectedCustomerId == 0 && string.IsNullOrWhiteSpace(txtPhone.Text)) return;
            var payload = new
            {
                KhachHangId = _selectedCustomerId,
                HoTen = txtFullName.Text.Trim(),
                SoDienThoai = txtPhone.Text.Trim(),

                Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim(),
                Cccd = string.IsNullOrWhiteSpace(txtIdentityCard.Text) ? null : txtIdentityCard.Text.Trim(),
                DiaChi = string.IsNullOrWhiteSpace(txtAddress.Text) ? null : txtAddress.Text.Trim()
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
            txtAddress.Clear();
            txtFullName.Focus();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            ++_searchVersion;
            _searchTimer.Stop();
            _searchTimer.Start();
        }

        // Designer bindings in the original project.
        private void dgvCustomers_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void dgvCustomers_DataError(object sender, DataGridViewDataErrorEventArgs e) { }
    }
}
