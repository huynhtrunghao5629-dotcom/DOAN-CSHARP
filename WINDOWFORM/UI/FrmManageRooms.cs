using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using HotelManagement.Desktop.Models;
using HotelManager.Services;

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
            await LoadLoaiPhongAsync();
            await LoadRoomsAsync();
        }

        private void InitStatusComboBox()
        {
            cboTrangThai.Items.Clear();
            cboTrangThai.DisplayMember = nameof(StatusOption.Nhan);
            cboTrangThai.ValueMember = nameof(StatusOption.Ma);

            // Đồng bộ mã trạng thái phòng chuẩn SQL / API
            cboTrangThai.Items.Add(new StatusOption("Trong", "Trống"));
            cboTrangThai.Items.Add(new StatusOption("DaNhanPhong", "Đang có khách"));
            cboTrangThai.Items.Add(new StatusOption("DaDat", "Đã đặt"));
            cboTrangThai.Items.Add(new StatusOption("DangBaoTri", "Đang bảo trì"));
            cboTrangThai.SelectedIndex = 0;
        }

        private void SetupDataGridView()
        {
            dgvRooms.AutoGenerateColumns = true;
            dgvRooms.DataBindingComplete += (sender, args) => TieuDeTiengViet.GanCho(dgvRooms);
            dgvRooms.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRooms.MultiSelect = false;
            dgvRooms.ReadOnly = true;
            dgvRooms.CellFormatting += (sender, args) =>
            {
                if (args.RowIndex >= 0 && args.ColumnIndex >= 0 &&
                    dgvRooms.Columns[args.ColumnIndex].DataPropertyName == "TrangThai")
                {
                    args.Value = TrangThaiTiengViet.Phong(args.Value?.ToString());
                    args.FormattingApplied = true;
                }
            };
        }

        private async Task LoadLoaiPhongAsync()
        {
            try
            {
                var client = BaseApiClient.GetClient();
                var response = await client.GetAsync("LoaiPhong");
                if (response.IsSuccessStatusCode)
                {
                    var dsLoaiPhong = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                    if (dsLoaiPhong != null && Controls.Find("cboLoaiPhong", true).Length > 0)
                    {
                        var cbo = Controls.Find("cboLoaiPhong", true)[0] as ComboBox;
                        if (cbo != null)
                        {
                            cbo.DataSource = dsLoaiPhong;
                            cbo.DisplayMember = "TenLoaiPhong";
                            cbo.ValueMember = "LoaiPhongId";
                        }
                    }
                }
            }
            catch
            {
                // Bỏ qua nếu endpoint LoaiPhong chưa sẵn sàng
            }
        }

        private async Task LoadRoomsAsync()
        {
            try
            {
                HttpClient client = BaseApiClient.GetClient();
                HttpResponseMessage response = await client.GetAsync("Phong");
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
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kết nối API: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int GetSelectedLoaiPhongId()
        {
            if (Controls.Find("cboLoaiPhong", true).Length > 0 && Controls.Find("cboLoaiPhong", true)[0] is ComboBox cbo && cbo.SelectedValue != null)
            {
                if (int.TryParse(cbo.SelectedValue.ToString(), out int id)) return id;
            }
            return 1; // Default LoaiPhongId fallback
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            var roomDto = new
            {
                SoPhong = txtSoPhong.Text.Trim(),
                LoaiPhongId = GetSelectedLoaiPhongId(),
                TrangThai = (cboTrangThai.SelectedItem as StatusOption)?.Ma ?? "Trong"
            };

            try
            {
                HttpClient client = BaseApiClient.GetClient();
                HttpResponseMessage response = await client.PostAsJsonAsync("Phong", roomDto);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Thêm phòng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadRoomsAsync();
                }
                else
                {
                    MessageBox.Show($"Thêm phòng thất bại. Mã lỗi API: {response.StatusCode}", "Lỗi API", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                PhongId = int.Parse(_selectedPhongId),
                SoPhong = txtSoPhong.Text.Trim(),
                LoaiPhongId = GetSelectedLoaiPhongId(),
                TrangThai = (cboTrangThai.SelectedItem as StatusOption)?.Ma ?? "Trong"
            };

            try
            {
                HttpClient client = BaseApiClient.GetClient();
                HttpResponseMessage response = await client.PutAsJsonAsync($"Phong/{_selectedPhongId}", roomDto);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Cập nhật phòng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadRoomsAsync();
                }
                else
                {
                    MessageBox.Show($"Cập nhật thất bại. Mã lỗi API: {response.StatusCode}", "Lỗi API", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                HttpClient client = BaseApiClient.GetClient();
                HttpResponseMessage response = await client.DeleteAsync($"Phong/{_selectedPhongId}");
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
                    _selectedPhongId = room.PhongId.ToString();
                    txtSoPhong.Text = room.SoPhong;
                    txtTenLoaiPhong.Text = room.TenLoaiPhong;
                    txtGiaPhong.Text = room.GiaPhong.ToString("G0");

                    for (int i = 0; i < cboTrangThai.Items.Count; i++)
                    {
                        if (cboTrangThai.Items[i] is StatusOption option &&
                            string.Equals(option.Ma, room.TrangThai, StringComparison.OrdinalIgnoreCase))
                        {
                            cboTrangThai.SelectedIndex = i;
                            break;
                        }
                    }
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

        private sealed class StatusOption
        {
            public string Ma { get; }
            public string Nhan { get; }
            public StatusOption(string ma, string nhan) { Ma = ma; Nhan = nhan; }
            public override string ToString() => Nhan;
        }
    }
}