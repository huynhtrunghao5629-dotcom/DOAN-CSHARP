using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
        private readonly ComboBox _cboLoaiPhong = new ComboBox();
        private readonly TextBox _txtHinhAnh = new TextBox();
        private readonly Button _btnChonAnh = new Button();

        public FrmManageRooms()
        {
            InitializeComponent();
        }

        private async void FrmManageRooms_Load(object sender, EventArgs e)
        {
            InitStatusComboBox();
            InitLoaiPhongComboBox();
            InitHinhAnhInput();
            SetupDataGridView();
            txtGiaPhong.ReadOnly = true;
            txtTenLoaiPhong.ReadOnly = true;
            await LoadLoaiPhongAsync();
            await LoadRoomsAsync();
        }

        private void InitStatusComboBox()
        {
            cboTrangThai.Items.Clear();
            cboTrangThai.DisplayMember = nameof(StatusOption.Nhan);
            cboTrangThai.ValueMember = nameof(StatusOption.Ma);

            cboTrangThai.Items.Add(new StatusOption("Trong", "Trống"));
            cboTrangThai.Items.Add(new StatusOption("DangSuDung", "Đang có khách"));
            cboTrangThai.Items.Add(new StatusOption("DaDat", "Đã đặt"));
            cboTrangThai.Items.Add(new StatusOption("DangBaoTri", "Đang bảo trì"));
            cboTrangThai.SelectedIndex = 0;
        }

        private void InitLoaiPhongComboBox()
        {
            _cboLoaiPhong.Name = "cboLoaiPhong";
            _cboLoaiPhong.Bounds = txtTenLoaiPhong.Bounds;
            _cboLoaiPhong.DropDownStyle = ComboBoxStyle.DropDownList;
            _cboLoaiPhong.DropDownWidth = Math.Max(_cboLoaiPhong.Width, 260);
            _cboLoaiPhong.DisplayMember = "TenLoai";
            _cboLoaiPhong.ValueMember = "LoaiPhongId";
            _cboLoaiPhong.SelectedIndexChanged += LoaiPhong_SelectedIndexChanged;
            txtTenLoaiPhong.Visible = false;
            grpInput.Controls.Add(_cboLoaiPhong);
            _cboLoaiPhong.BringToFront();
        }

        private void InitHinhAnhInput()
        {
            // Tự động đặt ô nhập tên file ảnh & nút chọn file ngay bên phải cboTrangThai
            var lblHinhAnh = new Label
            {
                Text = "Hình ảnh:",
                AutoSize = true,
                Location = new Point(cboTrangThai.Right + 20, cboTrangThai.Top + 4)
            };

            _txtHinhAnh.Name = "txtHinhAnh";
            _txtHinhAnh.Location = new Point(lblHinhAnh.Right + 65, cboTrangThai.Top);
            _txtHinhAnh.Width = 120;
            _txtHinhAnh.PlaceholderText = "vd: 101.jpg";

            _btnChonAnh.Name = "btnChonAnh";
            _btnChonAnh.Text = "...";
            _btnChonAnh.Location = new Point(_txtHinhAnh.Right + 4, cboTrangThai.Top - 1);
            _btnChonAnh.Size = new Size(32, _txtHinhAnh.Height + 2);
            _btnChonAnh.Click += BtnChonAnh_Click;

            grpInput.Controls.Add(lblHinhAnh);
            grpInput.Controls.Add(_txtHinhAnh);
            grpInput.Controls.Add(_btnChonAnh);
        }

        private void BtnChonAnh_Click(object? sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog
            {
                Title = "Chọn hình ảnh phòng",
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.webp"
            };
            string imagesFolder = GetImagesFolderPath();
            if (Directory.Exists(imagesFolder))
                dlg.InitialDirectory = imagesFolder;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                _txtHinhAnh.Text = Path.GetFileName(dlg.FileName);
            }
        }

        private static string GetImagesFolderPath()
        {
            // 1. Ưu tiên thư mục "images" nằm ngay cạnh file HotelManager.exe (chạy được trên mọi máy sau khi build/đóng gói)
            string localExeImages = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images");
            if (Directory.Exists(localExeImages))
                return localExeImages;

            // 2. Nếu chạy ở môi trường Dev từ thư mục clone Git bất kỳ, tự tìm ngược lên WEB\wwwroot\images
            DirectoryInfo? dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            while (dir != null)
            {
                string candidate = Path.Combine(dir.FullName, "WEB", "wwwroot", "images");
                if (Directory.Exists(candidate))
                    return candidate;
                dir = dir.Parent;
            }

            // 3. Fallback an toàn: tự tạo thư mục images cạnh file .exe nếu máy trắng hoàn toàn
            Directory.CreateDirectory(localExeImages);
            return localExeImages;
        }

        private static Image LoadRoomImageSafe(string? fileName)
{
    string folder = GetImagesFolderPath();
    string baseName = Path.GetFileNameWithoutExtension(fileName ?? string.Empty);
    string exactName = Path.GetFileName(fileName ?? string.Empty);

    // Danh sách các đường dẫn ưu tiên thử lần lượt
    var candidates = new List<string>();
    if (!string.IsNullOrWhiteSpace(exactName))
        candidates.Add(Path.Combine(folder, exactName));

    if (!string.IsNullOrWhiteSpace(baseName))
    {
        candidates.Add(Path.Combine(folder, baseName + ".jpg"));
        candidates.Add(Path.Combine(folder, baseName + ".jpeg"));
        candidates.Add(Path.Combine(folder, baseName + ".png"));
    }

    candidates.Add(Path.Combine(folder, "room-placeholder.jpg"));
    candidates.Add(Path.Combine(folder, "101.jpg")); // Dự phòng cuối cùng nếu thiếu placeholder

    foreach (var path in candidates)
    {
        if (!File.Exists(path)) continue;
        try
        {
            byte[] bytes = File.ReadAllBytes(path);
            using var ms = new MemoryStream(bytes);
            using var tempImg = Image.FromStream(ms);
            return new Bitmap(tempImg);
        }
        catch
        {
            // Bỏ qua nếu file bị sai định dạng (vd: WebP đội lốt .jpg) để thử file kế tiếp
        }
    }

    return new Bitmap(60, 40);
}

        private void SetupDataGridView()
        {
            grpInput.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            dgvRooms.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvRooms.AutoGenerateColumns = false;
            dgvRooms.Columns.Clear();
            dgvRooms.AllowUserToAddRows = false;
            dgvRooms.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRooms.MultiSelect = false;
            dgvRooms.ReadOnly = true;
            dgvRooms.RowTemplate.Height = 56;
            dgvRooms.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Chặn hộp thoại lỗi mặc định của DataGridView nếu có dữ liệu lạ
            dgvRooms.DataError += (s, e) => { e.ThrowException = false; };

            AddRoomColumn("PhongId", "Mã phòng", 60);

            // Cột hiển thị hình ảnh thu nhỏ (gắn DataPropertyName = "HinhAnh")
            var imgCol = new DataGridViewImageColumn
            {
                Name = "AnhHienThi",
                DataPropertyName = "HinhAnh",
                HeaderText = "Hình ảnh",
                ImageLayout = DataGridViewImageCellLayout.Zoom,
                MinimumWidth = 85,
                FillWeight = 85,
                ReadOnly = true
            };
            dgvRooms.Columns.Add(imgCol);

            AddRoomColumn("SoPhong", "Số phòng", 95);
            AddRoomColumn("TenLoaiPhong", "Loại phòng", 145);
            AddRoomColumn("GiaPhong", "Giá / đêm", 110, "N0");
            AddRoomColumn("TrangThai", "Trạng thái", 125);
            AddRoomColumn("HinhAnh", "File ảnh", 100);

            dgvRooms.CellFormatting += (sender, args) =>
            {
                if (args.RowIndex < 0 || args.ColumnIndex < 0) return;

                var colName = dgvRooms.Columns[args.ColumnIndex].Name;
                if (colName == "TrangThai")
                {
                    args.Value = TrangThaiTiengViet.Phong(args.Value?.ToString());
                    args.FormattingApplied = true;
                }
                else if (colName == "AnhHienThi")
                {
                    args.Value = LoadRoomImageSafe(args.Value?.ToString());
                    args.FormattingApplied = true;
                }
            };
        }

        private void AddRoomColumn(string property, string header, int width, string? format = null)
        {
            var column = new DataGridViewTextBoxColumn
            {
                Name = property,
                DataPropertyName = property,
                HeaderText = header,
                MinimumWidth = Math.Min(width, 75),
                FillWeight = width,
                ReadOnly = true
            };
            if (format != null) column.DefaultCellStyle.Format = format;
            dgvRooms.Columns.Add(column);
        }

        private async Task LoadLoaiPhongAsync()
        {
            try
            {
                using var response = await BaseApiClient.GetClient().GetAsync("LoaiPhong");
                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Không tải được loại phòng: HTTP {(int)response.StatusCode}", "Lỗi API");
                    return;
                }
                var loaiPhongs = await response.Content.ReadFromJsonAsync<List<LoaiPhongSelection>>() ?? new();
                _cboLoaiPhong.DataSource = null;
                _cboLoaiPhong.DataSource = loaiPhongs;
                _cboLoaiPhong.SelectedIndex = -1;
                if (loaiPhongs.Count == 0)
                    MessageBox.Show("Chưa có hạng phòng trong cơ sở dữ liệu.", "Thông báo");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không tải được loại phòng: {ex.Message}", "Lỗi API");
            }
        }

        private void LoaiPhong_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (sender is ComboBox combo && combo.SelectedItem is LoaiPhongSelection loai)
            {
                txtTenLoaiPhong.Text = loai.TenLoai;
                txtGiaPhong.Text = loai.GiaPhong.ToString("N0");
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
                    dgvRooms.ClearSelection();
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
            return _cboLoaiPhong.SelectedItem is LoaiPhongSelection loai ? loai.LoaiPhongId : 0;
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            string soPhong = txtSoPhong.Text.Trim();
            string hinhAnh = string.IsNullOrWhiteSpace(_txtHinhAnh.Text) ? $"{soPhong}.jpg" : _txtHinhAnh.Text.Trim();

            var roomDto = new
            {
                SoPhong = soPhong,
                LoaiPhongId = GetSelectedLoaiPhongId(),
                TrangThai = (cboTrangThai.SelectedItem as StatusOption)?.Ma ?? "Trong",
                HinhAnh = hinhAnh
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

            string soPhong = txtSoPhong.Text.Trim();
            string hinhAnh = string.IsNullOrWhiteSpace(_txtHinhAnh.Text) ? $"{soPhong}.jpg" : _txtHinhAnh.Text.Trim();

            var roomDto = new
            {
                PhongId = int.Parse(_selectedPhongId),
                SoPhong = soPhong,
                LoaiPhongId = GetSelectedLoaiPhongId(),
                TrangThai = (cboTrangThai.SelectedItem as StatusOption)?.Ma ?? "Trong",
                HinhAnh = hinhAnh
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
                    var detail = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Cập nhật thất bại. HTTP {(int)response.StatusCode}: {detail}", "Lỗi API", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    _txtHinhAnh.Text = room.HinhAnh ?? string.Empty;

                    if (room.LoaiPhongId > 0 && _cboLoaiPhong.DataSource != null)
                        _cboLoaiPhong.SelectedValue = room.LoaiPhongId;

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
            if (GetSelectedLoaiPhongId() <= 0)
            {
                MessageBox.Show("Vui lòng chọn loại phòng hợp lệ.", "Cảnh báo");
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtSoPhong.Text))
            {
                MessageBox.Show("Vui lòng nhập số phòng!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSoPhong.Focus();
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
            _txtHinhAnh.Clear();
            _cboLoaiPhong.SelectedIndex = -1;
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

        private sealed class LoaiPhongSelection
        {
            public int LoaiPhongId { get; set; }
            public string TenLoai { get; set; } = string.Empty;
            public decimal GiaPhong { get; set; }
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