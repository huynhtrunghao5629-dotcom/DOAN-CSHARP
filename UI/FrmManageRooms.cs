using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using HotelManagement.Desktop.Models;
using HotelManagement.Desktop.Services;
using System.Net.Http;
using System.Net.Http.Json;

namespace HotelManager.UI
{
    public partial class FrmManageRooms : Form
    {
        private readonly RoomService _roomService;
        private readonly HttpClient _client;
        private int _selectedRoomId = 0;
        private readonly UserSession _currentUser;

        // Danh sách lưu Loại phòng thật từ API
        private List<RoomTypeDto> _roomTypesList = new List<RoomTypeDto>();

        public FrmManageRooms()
        {
            InitializeComponent();
            _roomService = new RoomService();

            // Bỏ qua xác thực chứng chỉ SSL cho môi trường localhost
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };

            _client = new HttpClient(handler);
            _client.BaseAddress = new Uri("https://localhost:7203/");
        }

        public FrmManageRooms(UserSession userSession) : this()
        {
            _currentUser = userSession;
        }

        private async void FrmManageRooms_Load(object sender, EventArgs e)
        {
            SetupCustomLayout();
            this.Resize += (s, ev) => SetupCustomLayout();

            txtStatus.Items.Clear();
            txtStatus.Items.Add("Trống");
            txtStatus.Items.Add("Đang có khách");
            txtStatus.Items.Add("Bảo trì");
            txtStatus.DropDownStyle = ComboBoxStyle.DropDownList;

            // Load Loại Phòng từ API thật
            await LoadRoomTypesAsync();

            ApplyPermissions();
            await LoadRoomDataAsync();
        }

        private async Task LoadRoomTypesAsync()
        {
            try
            {
                // ĐÃ SỬA: Đường dẫn đúng là "api/LoaiPhong" (không có chữ Controller)
                var response = await _client.GetFromJsonAsync<List<RoomTypeDto>>("api/LoaiPhong");

                if (response != null && response.Count > 0)
                {
                    _roomTypesList = response;

                    // Gán vào ComboBox
                    cboRoomType.DataSource = null;
                    cboRoomType.DataSource = _roomTypesList;
                    cboRoomType.DisplayMember = "RoomTypeName"; // Hiển thị tên (TenLoai)
                    cboRoomType.ValueMember = "RoomTypeId";     // Lấy giá trị ID (LoaiPhongId)
                    cboRoomType.DropDownStyle = ComboBoxStyle.DropDownList;
                }
                else
                {
                    MessageBox.Show("Không tìm thấy loại phòng nào trong CSDL!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách Loại Phòng: {ex.Message}", "Lỗi kết nối API", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupCustomLayout()
        {
            if (groupBox1 != null && dgvRooms != null)
            {
                groupBox1.Dock = DockStyle.Top;
                groupBox1.Height = 220;

                dgvRooms.Dock = DockStyle.None;
                dgvRooms.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

                int spacing = 12;
                dgvRooms.Top = groupBox1.Bottom + spacing;
                dgvRooms.Left = 12;
                dgvRooms.Width = this.ClientSize.Width - 24;
                dgvRooms.Height = this.ClientSize.Height - dgvRooms.Top - 12;

                dgvRooms.BringToFront();
            }
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            ClearForm();

            // Đưa con trỏ về ô nhập số phòng để sẵn sàng nhập liệu mới
            txtRoomNumber.Focus();
        }
        private void ApplyPermissions()
        {
            if (_currentUser != null)
            {
                bool isAdmin = "Admin".Equals(_currentUser.Role, StringComparison.OrdinalIgnoreCase);

                txtRoomNumber.Enabled = isAdmin;
                txtPrice.Enabled = isAdmin;
                txtStatus.Enabled = isAdmin;
                if (cboRoomType != null) cboRoomType.Enabled = isAdmin;

                btnAdd.Enabled = isAdmin;
                btnAddRoom.Enabled = isAdmin;
                btnDelete.Enabled = isAdmin;
            }
            else
            {
                txtRoomNumber.Enabled = false;
                txtPrice.Enabled = false;
                txtStatus.Enabled = false;
                if (cboRoomType != null) cboRoomType.Enabled = false;

                btnAdd.Enabled = false;
                btnAddRoom.Enabled = false;
                btnDelete.Enabled = false;
            }

            txtSearch.Enabled = true;
        }

        private async Task LoadRoomDataAsync()
        {
            try
            {
                var rooms = await _roomService.GetAllRoomsAsync();

                if (rooms == null)
                {
                    MessageBox.Show("Danh sách phòng trả về null từ Service!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                dgvRooms.DataSource = null;
                dgvRooms.DataSource = rooms;

                FormatDataGridView();

                dgvRooms.Refresh();
                dgvRooms.Update();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chi tiết lỗi tải phòng: " + ex.ToString(), "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatDataGridView()
        {
            if (dgvRooms == null || dgvRooms.Columns.Count == 0) return;

            dgvRooms.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvRooms.ColumnHeadersVisible = true;

            if (dgvRooms.Columns["RoomId"] != null)
            {
                dgvRooms.Columns["RoomId"].HeaderText = "Mã Phòng";
                dgvRooms.Columns["RoomId"].DataPropertyName = "RoomId";
                dgvRooms.Columns["RoomId"].ReadOnly = true;
                dgvRooms.Columns["RoomId"].DefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
            }
            if (dgvRooms.Columns["RoomNumber"] != null)
            {
                dgvRooms.Columns["RoomNumber"].HeaderText = "Số Phòng";
                dgvRooms.Columns["RoomNumber"].DataPropertyName = "RoomNumber";
            }
            if (dgvRooms.Columns["RoomTypeName"] != null)
            {
                dgvRooms.Columns["RoomTypeName"].HeaderText = "Loại Phòng";
                dgvRooms.Columns["RoomTypeName"].DataPropertyName = "RoomTypeName"; // Khớp với RoomTypeName trong RoomDto
            }
            if (dgvRooms.Columns["PricePerNight"] != null)
            {
                dgvRooms.Columns["PricePerNight"].HeaderText = "Giá / Đêm (VNĐ)";
                dgvRooms.Columns["PricePerNight"].DataPropertyName = "PricePerNight";
                dgvRooms.Columns["PricePerNight"].DefaultCellStyle.Format = "N0";
            }
            if (dgvRooms.Columns["Status"] != null)
            {
                dgvRooms.Columns["Status"].HeaderText = "Trạng Thái";
                dgvRooms.Columns["Status"].DataPropertyName = "Status";
            }

            if (dgvRooms.Columns["RoomTypeId"] != null) dgvRooms.Columns["RoomTypeId"].Visible = false;

            dgvRooms.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRooms.MultiSelect = false;
            dgvRooms.AllowUserToAddRows = false;
            dgvRooms.ReadOnly = true;
        }

        private void dgvRooms_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvRooms.Rows[e.RowIndex];
                _selectedRoomId = Convert.ToInt32(row.Cells["RoomId"].Value);
                txtRoomNumber.Text = row.Cells["RoomNumber"].Value?.ToString();
                txtPrice.Text = row.Cells["PricePerNight"].Value?.ToString();

                string rawStatus = row.Cells["Status"].Value?.ToString();

                switch (rawStatus)
                {
                    case "Available":
                    case "Trống":
                    case "Trong":
                        txtStatus.SelectedItem = "Trống";
                        break;
                    case "Occupied":
                    case "Đang có khách":
                    case "Dang co khach":
                        txtStatus.SelectedItem = "Đang có khách";
                        break;
                    case "Maintenance":
                    case "Bao tri":
                    case "Bảo trì":
                        txtStatus.SelectedItem = "Bảo trì";
                        break;
                    default:
                        txtStatus.SelectedIndex = -1;
                        break;
                }

                // Cập nhật giá trị cho ComboBox Loại Phòng
                if (row.Cells["RoomTypeId"].Value != null)
                {
                    int typeId = Convert.ToInt32(row.Cells["RoomTypeId"].Value);
                    cboRoomType.SelectedValue = typeId;
                }
            }
        }
        // Hàm tải lại danh sách phòng (bạn thay thế tên hàm nạp dữ liệu chính của bạn vào đây nếu khác)
        private async Task LoadRoomsAsync()
        {
            var rooms = await _roomService.GetAllRoomsAsync();
            dgvRooms.DataSource = rooms;
            FormatDataGridView();
        }

        private void ClearForm()
        {
            _selectedRoomId = 0; // QUAN TRỌNG: Bắt buộc đưa về 0 để form hiểu đây là trạng thái Thêm mới, không bị lỗi trùng số phòng
            txtRoomNumber.Clear();
            txtPrice.Clear();

            if (txtStatus.Items.Count > 0)
                txtStatus.SelectedIndex = -1;

            if (cboRoomType.Items.Count > 0)
                cboRoomType.SelectedIndex = -1;

            // Bỏ chọn dòng trên DataGridView nếu có
            if (dgvRooms.SelectedRows.Count > 0)
                dgvRooms.ClearSelection();
        }
        // 1. NÚT THÊM MỚI PHÒNG (Gắn sự kiện cho nút Thêm mới của bạn)
        private async void btnAddRoom_Click(object sender, EventArgs e)
        {
            var newRoom = new RoomDto
            {
                RoomId = 0,
                RoomNumber = txtRoomNumber.Text.Trim(),
                PricePerNight = decimal.TryParse(txtPrice.Text, out var price) ? price : 0,
                Status = txtStatus.SelectedItem?.ToString() ?? "Trống",
                RoomTypeId = cboRoomType.SelectedValue is int typeId ? typeId : 1
            };

            var validation = ValidationHelper.ValidateRoom(newRoom);
            if (!validation.IsValid)
            {
                MessageBox.Show(validation.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Gọi hàm Thêm mới (POST) riêng biệt
            bool success = await _roomService.CreateRoomAsync(newRoom);
            if (success)
            {
                MessageBox.Show("Thêm phòng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadRoomDataAsync();
                ClearForm();
            }
            else
            {
                MessageBox.Show("Thêm phòng thất bại! Số phòng có thể đã tồn tại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 2. NÚT CẬP NHẬT / SỬA PHÒNG (Gắn sự kiện cho nút Cập nhật/Sửa của bạn)
        private async void btnUpdateRoom_Click(object sender, EventArgs e)
        {
            if (_selectedRoomId <= 0)
            {
                MessageBox.Show("Vui lòng chọn phòng cần sửa từ bảng bên dưới!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var updatedRoom = new RoomDto
            {
                RoomId = _selectedRoomId,
                RoomNumber = txtRoomNumber.Text.Trim(),
                PricePerNight = decimal.TryParse(txtPrice.Text, out var price) ? price : 0,
                Status = txtStatus.SelectedItem?.ToString() ?? "Trống",
                RoomTypeId = cboRoomType.SelectedValue is int typeId ? typeId : 1
            };

            var validation = ValidationHelper.ValidateRoom(updatedRoom);
            if (!validation.IsValid)
            {
                MessageBox.Show(validation.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool success = await _roomService.UpdateRoomAsync(updatedRoom);
            if (success)
            {
                MessageBox.Show("Cập nhật phòng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                await LoadRoomDataAsync();
                dgvRooms.Update();
                dgvRooms.Refresh();
                ClearForm(); // Đưa _selectedRoomId về 0 và làm sạch ô nhập
            }
            else
            {
                MessageBox.Show("Cập nhật thất bại! Số phòng có thể bị trùng với phòng khác.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtRoomNumber.Text) || string.IsNullOrWhiteSpace(txtPrice.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ số phòng và giá phòng!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(txtPrice.Text, out decimal price))
                {
                    MessageBox.Show("Giá phòng phải là một số hợp lệ!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (cboRoomType.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng chọn loại phòng!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var room = new RoomDto
                {
                    RoomId = _selectedRoomId,
                    RoomNumber = txtRoomNumber.Text.Trim(),
                    PricePerNight = price,
                    Status = txtStatus.SelectedItem?.ToString() ?? "Trống",
                    RoomTypeId = (int)cboRoomType.SelectedValue
                };

                bool isSuccess = _selectedRoomId > 0
                    ? await _roomService.UpdateRoomAsync(room)
                    : await _roomService.CreateRoomAsync(room);

                if (isSuccess)
                {
                    MessageBox.Show(
                        _selectedRoomId > 0
                            ? "Cập nhật phòng thành công!"
                            : "Thêm phòng thành công!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    await LoadRoomDataAsync(); // Tải lại danh sách mới nhất cho bảng
                    ClearForm(); // Reset form và đưa _selectedRoomId về 0
                }
                else
                {
                    MessageBox.Show(
                        "Thực hiện thất bại! Kiểm tra lại thông tin hoặc số phòng bị trùng.",
                        "Lỗi API",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }





        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedRoomId == 0)
            {
                MessageBox.Show("Vui lòng chọn phòng cần xóa từ danh sách!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show($"Bạn có chắc chắn muốn xóa phòng ID: {_selectedRoomId} không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                try
                {
                    bool isSuccess = await _roomService.DeleteRoomAsync(_selectedRoomId);
                    if (isSuccess)
                    {
                        MessageBox.Show("Đã xóa phòng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        btnAddRoom_Click(sender, e);
                        ClearForm();
                    }
                    else
                    {
                        MessageBox.Show("Xóa phòng thất bại! Phòng này có thể đang có người đặt.", "Lỗi API", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xóa phòng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string keyword = txtSearch.Text.Trim().ToLower();
                var allRooms = await _roomService.GetAllRoomsAsync();

                if (string.IsNullOrEmpty(keyword))
                {
                    dgvRooms.DataSource = allRooms;
                }
                else
                {
                    var filteredList = allRooms.Where(r => r.RoomNumber != null && r.RoomNumber.ToLower().Contains(keyword)).ToList();
                    dgvRooms.DataSource = filteredList;
                }

                FormatDataGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tìm kiếm: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvRooms_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        // CÁC HÀM SỰ KIỆN KẾT NỐI TỪ DESIGNER
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void txtRoomNumber_TextChanged(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void groupBox1_Enter(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void label4_Click(object sender, EventArgs e) { }
        private void txtPrice_TextChanged(object sender, EventArgs e) { }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) { }
        private void label5_Click(object sender, EventArgs e) { }

        private void btnManageRoomTypes_Click(object sender, EventArgs e)
        {
            FrmManageRoomTypes frm = new FrmManageRoomTypes();
            frm.ShowDialog();

            _ = LoadRoomTypesAsync();
        }
    }
}