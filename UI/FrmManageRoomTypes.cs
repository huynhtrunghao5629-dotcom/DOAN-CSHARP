using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using HotelManagement.Desktop.Models;
using HotelManagement.Desktop.Services;
using static HotelManagement.Desktop.Services.RoomService;

namespace HotelManager.UI
{
    public partial class FrmManageRoomTypes : Form
    {
        private readonly RoomTypeService _roomTypeService;
        private int _selectedRoomTypeId = 0;

        public FrmManageRoomTypes()
        {
            InitializeComponent();
            _roomTypeService = new RoomTypeService();
        }

        private async void FrmManageRoomTypes_Load(object sender, EventArgs e)
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var list = await _roomTypeService.GetAllRoomTypesAsync();
                dgvRoomTypes.DataSource = null;
                dgvRoomTypes.DataSource = list;
                FormatGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatGrid()
        {
            if (dgvRoomTypes.Columns.Count == 0) return;
            dgvRoomTypes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            if (dgvRoomTypes.Columns["RoomTypeId"] != null)
            {
                dgvRoomTypes.Columns["RoomTypeId"].HeaderText = "Mã Loại";
                dgvRoomTypes.Columns["RoomTypeId"].DataPropertyName = "RoomTypeId";
            }
            if (dgvRoomTypes.Columns["RoomTypeName"] != null)
            {
                dgvRoomTypes.Columns["RoomTypeName"].HeaderText = "Tên Loại Phòng";
                dgvRoomTypes.Columns["RoomTypeName"].DataPropertyName = "RoomTypeName";
            }
            if (dgvRoomTypes.Columns["PricePerNight"] != null)
            {
                dgvRoomTypes.Columns["PricePerNight"].HeaderText = "Giá / Đêm (VNĐ)";
                dgvRoomTypes.Columns["PricePerNight"].DataPropertyName = "PricePerNight";
                dgvRoomTypes.Columns["PricePerNight"].DefaultCellStyle.Format = "N0";
            }

            dgvRoomTypes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRoomTypes.ReadOnly = true;
            dgvRoomTypes.AllowUserToAddRows = false;
        }

        private void dgvRoomTypes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dgvRoomTypes.Rows[e.RowIndex];
                _selectedRoomTypeId = Convert.ToInt32(row.Cells["RoomTypeId"].Value);
                txtTenLoai.Text = row.Cells["RoomTypeName"].Value?.ToString();
                txtGiaPhong.Text = row.Cells["PricePerNight"].Value?.ToString();
            }
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenLoai.Text) || !decimal.TryParse(txtGiaPhong.Text, out decimal price))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tên loại phòng và giá hợp lệ!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var newType = new RoomTypeDto
            {
                RoomTypeName = txtTenLoai.Text.Trim(),
                PricePerNight = price
            };

            bool success = await _roomTypeService.CreateRoomTypeAsync(newType);
            if (success)
            {
                MessageBox.Show("Thêm loại phòng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm();
                await LoadDataAsync();
            }
            else
            {
                MessageBox.Show("Thêm thất bại: " + _roomTypeService.LastError, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            if (_selectedRoomTypeId <= 0)
            {
                MessageBox.Show("Vui lòng chọn loại phòng cần sửa từ bảng!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTenLoai.Text) || !decimal.TryParse(txtGiaPhong.Text, out decimal price))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tên loại phòng và giá hợp lệ!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var updatedType = new RoomTypeDto
            {
                RoomTypeId = _selectedRoomTypeId,
                RoomTypeName = txtTenLoai.Text.Trim(),
                PricePerNight = price
            };

            bool success = await _roomTypeService.UpdateRoomTypeAsync(updatedType);
            if (success)
            {
                MessageBox.Show("Cập nhật giá và loại phòng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm();
                await LoadDataAsync();
            }
            else
            {
                MessageBox.Show("Cập nhật thất bại: " + _roomTypeService.LastError, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedRoomTypeId <= 0)
            {
                MessageBox.Show("Vui lòng chọn loại phòng cần xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show("Bạn có chắc chắn muốn xóa loại phòng này không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                bool success = await _roomTypeService.DeleteRoomTypeAsync(_selectedRoomTypeId);
                if (success)
                {
                    MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                    await LoadDataAsync();
                }
                else
                {
                    MessageBox.Show("Xóa thất bại (Loại phòng có thể đang được sử dụng bởi các phòng khác): " + _roomTypeService.LastError, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            _selectedRoomTypeId = 0;
            txtTenLoai.Clear();
            txtGiaPhong.Clear();
            txtTenLoai.Focus();
        }
    }
}