using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HotelManager.UI
{
    public partial class FrmManageRooms : Form
    {
        private sealed class PhongApiDto
        {
            public int PhongId { get; set; }
            public string SoPhong { get; set; } = "";
            public string TenLoaiPhong { get; set; } = "";
            public decimal GiaPhong { get; set; }
            public string TrangThai { get; set; } = "";
            public string TrangThaiHienThi => HienThiTrangThai(TrangThai);
        }

        private static readonly HttpClient Api = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7203/"),
            Timeout = TimeSpan.FromSeconds(12)
        };
        private readonly BindingSource _binding = new BindingSource();
        private List<PhongApiDto> _rooms = new List<PhongApiDto>();
        private DataGridView _grid = null!;
        private TextBox _txtSearch = null!;
        private Label _lblStatus = null!;
        private Label _lblCount = null!;
        private Button _btnReload = null!;

        public FrmManageRooms()
        {
            InitializeComponent();
            BuildInterface();
        }

        private void BuildInterface()
        {
            Text = "Quản lý phòng";
            BackColor = Color.FromArgb(246, 248, 247);
            Font = new Font("Segoe UI", 10);
            MinimumSize = new Size(760, 470);

            var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(24), ColumnCount = 1, RowCount = 4 };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 85));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 66));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));

            var header = new Panel { Dock = DockStyle.Fill };
            header.Controls.Add(new Label {
                Text = "QUẢN LÝ PHÒNG", Font = new Font("Segoe UI", 21, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 68, 53), Location = new Point(0, 1), AutoSize = true
            });
            header.Controls.Add(new Label {
                Text = "Danh sách phòng được tải trực tiếp từ Web API của khách sạn",
                ForeColor = Color.DimGray, Location = new Point(2, 46), AutoSize = true
            });
            root.Controls.Add(header, 0, 0);

            var actions = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1 };
            actions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            actions.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
            actions.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 155));
            _txtSearch = new TextBox { Dock = DockStyle.Fill, PlaceholderText = "Tìm số phòng hoặc loại phòng...", Margin = new Padding(0, 9, 12, 14) };
            _txtSearch.TextChanged += (_, _) => Filter();
            _btnReload = MakeButton("Tải lại", true);
            _btnReload.Click += async (_, _) => await LoadRoomsAsync();
            var btnInfo = MakeButton("Xem chi tiết", false);
            btnInfo.Click += (_, _) => ShowSelected();
            actions.Controls.Add(_txtSearch, 0, 0);
            actions.Controls.Add(_btnReload, 1, 0);
            actions.Controls.Add(btnInfo, 2, 0);
            root.Controls.Add(actions, 0, 1);

            _grid = new DataGridView {
                Dock = DockStyle.Fill, BackgroundColor = Color.White, BorderStyle = BorderStyle.None,
                AutoGenerateColumns = false, ReadOnly = true, AllowUserToAddRows = false,
                AllowUserToDeleteRows = false, MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowTemplate = { Height = 42 }, ColumnHeadersHeight = 43,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                EnableHeadersVisualStyles = false, GridColor = Color.FromArgb(228, 235, 231)
            };
            _grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 68, 53);
            _grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            _grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            _grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(214, 230, 220);
            _grid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(29, 57, 45);
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Mã", DataPropertyName = "PhongId", FillWeight = 35 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Số phòng", DataPropertyName = "SoPhong", FillWeight = 75 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Loại phòng", DataPropertyName = "TenLoaiPhong", FillWeight = 120 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Giá / đêm", DataPropertyName = "GiaPhong", FillWeight = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight } });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Trạng thái", DataPropertyName = "TrangThaiHienThi", FillWeight = 85 });
            _grid.DataSource = _binding;
            root.Controls.Add(_grid, 0, 2);

            var footer = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2 };
            footer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 75));
            footer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            _lblStatus = new Label { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, ForeColor = Color.DimGray, Text = "Đang chờ tải dữ liệu..." };
            _lblCount = new Label { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight, ForeColor = Color.FromArgb(30, 68, 53) };
            footer.Controls.Add(_lblStatus, 0, 0);
            footer.Controls.Add(_lblCount, 1, 0);
            root.Controls.Add(footer, 0, 3);
            Controls.Add(root);
        }

        // Chuẩn hóa cách HIỂN THỊ; giữ nguyên giá trị trạng thái từ API/SQL.
        private static string HienThiTrangThai(string? value)
        {
            var key = (value ?? "").Trim().ToLowerInvariant();
            return key switch
            {
                "dadat" or "đã đặt" => "Đã đặt",
                "choxacnhan" or "chờ xác nhận" => "Chờ xác nhận",
                "trong" or "trống" => "Trống",
                "dangcokhach" or "đang có khách" => "Đang có khách",
                "baotri" or "bảo trì" => "Bảo trì",
                "dahuy" or "đã hủy" => "Đã hủy",
                "dathanhtoan" or "đã thanh toán" => "Đã thanh toán",
                _ => string.IsNullOrWhiteSpace(value) ? "Chưa xác định" : value.Trim()
            };
        }

        private static Button MakeButton(string text, bool primary)
        {
            var b = new Button { Text = text, Dock = DockStyle.Fill, Margin = new Padding(3, 5, 3, 13), FlatStyle = FlatStyle.Flat,
                BackColor = primary ? Color.FromArgb(30, 68, 53) : Color.White,
                ForeColor = primary ? Color.White : Color.FromArgb(30, 68, 53), Cursor = Cursors.Hand };
            b.FlatAppearance.BorderColor = Color.FromArgb(30, 68, 53);
            return b;
        }

        private async void FrmManageRooms_Load(object sender, EventArgs e)
        {
            await LoadRoomsAsync();
        }

        private async Task LoadRoomsAsync()
        {
            _btnReload.Enabled = false;
            _lblStatus.Text = "Đang kết nối API...";
            try
            {
                // Endpoint GET api/Phong đang có trong QuanLyKhachSanAPI.
                var rooms = await Api.GetFromJsonAsync<List<PhongApiDto>>("api/Phong");
                _rooms = rooms ?? new List<PhongApiDto>();
                Filter();
                _lblStatus.Text = "Đã lấy dữ liệu phòng từ API.";
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException || ex is System.Text.Json.JsonException)
            {
                _rooms.Clear();
                Filter();
                _lblStatus.Text = "Không kết nối được API. Hãy chạy API rồi bấm Tải lại.";
                MessageBox.Show($"Không thể tải phòng từ https://localhost:7203/api/Phong\n\n{ex.Message}",
                    "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                _btnReload.Enabled = true;
            }
        }

        private void Filter()
        {
            var key = _txtSearch.Text.Trim();
            var items = _rooms.Where(r => string.IsNullOrEmpty(key)
                || r.SoPhong.Contains(key, StringComparison.OrdinalIgnoreCase)
                || r.TenLoaiPhong.Contains(key, StringComparison.OrdinalIgnoreCase)).ToList();
            _binding.DataSource = items;
            _lblCount.Text = $"{items.Count} / {_rooms.Count} phòng";

        }

        private void ShowSelected()
        {
            if (_binding.Current is not PhongApiDto room)
            {
                MessageBox.Show("Hãy chọn một phòng trong bảng.", "Thông báo");
                return;
            }
            MessageBox.Show($"Phòng: {room.SoPhong}\nLoại: {room.TenLoaiPhong}\nGiá: {room.GiaPhong:N0} đ/đêm\nTrạng thái: {room.TrangThaiHienThi}",
                "Chi tiết phòng", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
