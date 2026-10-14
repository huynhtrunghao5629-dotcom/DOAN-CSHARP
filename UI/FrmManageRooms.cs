using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using HotelManagement.Desktop.Models;
using HotelManagement.Desktop.Services;

namespace HotelManager.UI
{
    public partial class FrmManageRooms : Form
    {
        private readonly RoomService _roomService;
        private int _selectedRoomId = 0; // Lưu ID phòng đang chọn
        private readonly UserSession _currentUser; // Lưu thông tin người dùng đăng nhập

        // Constructor mặc định dành cho Designer
        public FrmManageRooms()
        {
            InitializeComponent();
            _roomService = new RoomService();
        }

        // Constructor chính nhận UserSession từ FrmMain
        public FrmManageRooms(UserSession userSession) : this()
        {
            _currentUser = userSession;
        }

        // Sự kiện khi Form tải lên
        private async void FrmManageRooms_Load(object sender, EventArgs e)
        {
            // 1. Cấu hình bố cục vị trí và khoảng cách chuẩn xác
            SetupCustomLayout();
            this.Resize += (s, ev) => SetupCustomLayout(); // Tự động canh lại khi phóng to/thu nhỏ form

            // 2. Nạp danh sách lựa chọn Trạng thái vào ComboBox
            txtStatus.Items.Clear();
            txtStatus.Items.Add("Available");
            txtStatus.Items.Add("Occupied");
            txtStatus.Items.Add("Maintenance");
            txtStatus.DropDownStyle = ComboBoxStyle.DropDownList;

            // 3. Áp dụng phân quyền và nạp dữ liệu
            ApplyPermissions();
            await LoadRoomDataAsync();
        }

        // Hàm canh khoảng cách và vị trí cho bảng dữ liệu
        private void SetupCustomLayout()
        {
            if (groupBox1 != null && dgvRooms != null)
            {
                groupBox1.Dock = DockStyle.Top;
                groupBox1.Height = 220;

                dgvRooms.Dock = DockStyle.None;
                dgvRooms.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

                int spacing = 12; // Khoảng cách hở giữa GroupBox và DataGridView
                dgvRooms.Top = groupBox1.Bottom + spacing;
                dgvRooms.Left = 12;
                dgvRooms.Width = this.ClientSize.Width - 24;
                dgvRooms.Height = this.ClientSize.Height - dgvRooms.Top - 12;

                dgvRooms.BringToFront();
            }
        }

        // Kiểm tra và áp dụng phân quyền
        private void ApplyPermissions()
        {
            if (_currentUser != null)
            {
                bool isAdmin = "Admin".Equals(_currentUser.Role, StringComparison.OrdinalIgnoreCase);

                txtRoomNumber.Enabled = isAdmin;
                txtPrice.Enabled = isAdmin;
                txtStatus.Enabled = isAdmin;

                btnAdd.Enabled = isAdmin;
                btnSave.Enabled = isAdmin;
                btnDelete.Enabled = isAdmin;
            }
            else
            {
                txtRoomNumber.Enabled = false;
                txtPrice.Enabled = false;
                txtStatus.Enabled = false;
                btnAdd.Enabled = false;
                btnSave.Enabled = false;
                btnDelete.Enabled = false;
            }

            txtSearch.Enabled = true;
        }

        // Hàm lấy danh sách phòng từ Service và đổ vào DataGridView
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

                // Gọi hàm định dạng tiêu đề, ẩn cột thừa và định dạng hiển thị
                FormatDataGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chi tiết lỗi tải phòng: " + ex.ToString(), "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Hàm định dạng hiển thị cho DataGridView (Tiếng Việt & UX)
        private void FormatDataGridView()
        {
            if (dgvRooms == null || dgvRooms.Columns.Count == 0) return;

            // Tự động giãn cột lấp đầy chiều rộng bảng
            dgvRooms.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvRooms.ColumnHeadersVisible = true;

            // Đổi tên tiêu đề các cột sang Tiếng Việt
            if (dgvRooms.Columns["RoomId"] != null)
            {
                dgvRooms.Columns["RoomId"].HeaderText = "Mã Phòng";
                dgvRooms.Columns["RoomId"].ReadOnly = true;
                dgvRooms.Columns["RoomId"].DefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
            }
            if (dgvRooms.Columns["RoomNumber"] != null) dgvRooms.Columns["RoomNumber"].HeaderText = "Số Phòng";
            if (dgvRooms.Columns["RoomTypeName"] != null) dgvRooms.Columns["RoomTypeName"].HeaderText = "Loại Phòng";
            if (dgvRooms.Columns["PricePerNight"] != null)
            {
                dgvRooms.Columns["PricePerNight"].HeaderText = "Giá / Đêm (VNĐ)";
                dgvRooms.Columns["PricePerNight"].DefaultCellStyle.Format = "N0"; // Định dạng phân cách nghìn
            }
            if (dgvRooms.Columns["Status"] != null) dgvRooms.Columns["Status"].HeaderText = "Trạng Thái";

            // Ẩn các cột ID không cần thiết cho người dùng thấy
            if (dgvRooms.Columns["RoomTypeId"] != null) dgvRooms.Columns["RoomTypeId"].Visible = false;

            // Cấu hình hành vi bảng
            dgvRooms.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // Click chọn nguyên dòng
            dgvRooms.MultiSelect = false; // Chỉ cho chọn 1 dòng
            dgvRooms.AllowUserToAddRows = false; // Bỏ dòng trống dưới cùng
            dgvRooms.ReadOnly = true; // Không cho chỉnh sửa trực tiếp trên ô
        }

        // Đổ dữ liệu từ dòng được chọn lên các control
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
                        txtStatus.SelectedItem = "Trống";
                        break;
                    case "Occupied":
                    case "Đang có khách":
                        txtStatus.SelectedItem = "Đang có khách";
                        break;
                    case "Maintenance":
                    case "Đang bảo trì":
                        txtStatus.SelectedItem = "Đang bảo trì";
                        break;
                    case "Cleaning":
                    case "Đang dọn dẹp":
                        txtStatus.SelectedItem = "Đang dọn dẹp";
                        break;
                    default:
                        txtStatus.SelectedIndex = -1;
                        break;
                }
            }
        }

        // Nút Thêm mới (Reset form)
        private void btnAdd_Click(object sender, EventArgs e)
        {
            _selectedRoomId = 0;
            txtRoomNumber.Clear();
            txtPrice.Clear();

            txtStatus.SelectedIndex = -1;
            txtStatus.Text = string.Empty;

            txtRoomNumber.Focus();
        }

        // Nút Lưu (Thêm hoặc Cập nhật)
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

                string selectedText = txtStatus.SelectedItem?.ToString() ?? txtStatus.Text.Trim();

                RoomStatus statusEnum = RoomStatus.Available;
                if (!string.IsNullOrEmpty(selectedText) && Enum.TryParse(selectedText, true, out RoomStatus parsedStatus))
                {
                    statusEnum = parsedStatus;
                }

                var room = new RoomDto
                {
                    RoomId = _selectedRoomId,
                    RoomNumber = txtRoomNumber.Text.Trim(),
                    RoomTypeName = "Standard",
                    PricePerNight = price,
                    Status = statusEnum
                };

                await _roomService.SaveRoomAsync(room);
                MessageBox.Show("Lưu thông tin phòng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                btnAdd_Click(sender, e);
                await LoadRoomDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu phòng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Nút Xóa phòng
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
                    await _roomService.DeleteRoomAsync(_selectedRoomId);
                    MessageBox.Show("Đã xóa phòng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    btnAdd_Click(sender, e);
                    await LoadRoomDataAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xóa phòng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Sự kiện tìm kiếm tự động
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

        // Xử lý lỗi định dạng dữ liệu trên DataGridView
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
    }
}