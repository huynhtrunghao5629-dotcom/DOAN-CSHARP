using HotelManagement.Desktop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HotelManager.UI
{
    public partial class FrmManageBookings : Form
    {
        private List<BookingViewModel> _allBookings = new List<BookingViewModel>();

        public FrmManageBookings()
        {
            InitializeComponent();
        }

        private async void FrmManageBookings_Load(object sender, EventArgs e)
        {
            InitFilterComboBox();
            SetupDataGridView();
            await LoadBookingsAsync();
        }

        // ====================================================================
        // 1. KHỞI TẠO CẤU HÌNH GIAO DIỆN
        // ====================================================================
        private void InitFilterComboBox()
        {
            cboStatusFilter.Items.Clear();
            cboStatusFilter.Items.Add("Tất cả");
            cboStatusFilter.Items.Add("Chờ xác nhận");
            cboStatusFilter.Items.Add("Đã xác nhận");
            cboStatusFilter.Items.Add("Đã nhận phòng (Đang ở)");
            cboStatusFilter.Items.Add("Đã hủy");
            cboStatusFilter.SelectedIndex = 0;
        }

        private void SetupDataGridView()
        {
            dgvBookings.AutoGenerateColumns = false;
            dgvBookings.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvBookings.MultiSelect = false;
            dgvBookings.ReadOnly = true;
        }

        // ====================================================================
        // 2. TẢI DỮ LIỆU ĐƠN ĐẶT PHÒNG TỪ API
        // ====================================================================
        private async Task LoadBookingsAsync()
        {
            try
            {
                btnRefresh.Enabled = false;

                // Sử dụng BaseApiClient để tự động đính kèm Token xác thực
                using (HttpClient client = BaseApiClient.GetClient())
                {
                    HttpResponseMessage response = await client.GetAsync("api/DonDatPhong");
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadFromJsonAsync<List<BookingViewModel>>();
                        _allBookings = data ?? new List<BookingViewModel>();
                        ApplyFilter();
                    }
                    else
                    {
                        MessageBox.Show($"Không thể tải danh sách đơn đặt phòng. Mã lỗi: {response.StatusCode}",
                                        "Lỗi API", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kết nối API: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnRefresh.Enabled = true;
            }
        }

        // Lọc danh sách theo Trạng thái trên ComboBox
        private void ApplyFilter()
        {
            string selectedFilter = cboStatusFilter.SelectedItem?.ToString() ?? "Tất cả";
            IEnumerable<BookingViewModel> filtered = _allBookings;

            switch (selectedFilter)
            {
                case "Chờ xác nhận":
                    filtered = _allBookings.Where(b => b.TrangThai == "ChoXacNhan");
                    break;
                case "Đã xác nhận":
                    filtered = _allBookings.Where(b => b.TrangThai == "DaXacNhan");
                    break;
                case "Đã nhận phòng (Đang ở)":
                    filtered = _allBookings.Where(b => b.TrangThai == "DaNhanPhong");
                    break;
                case "Đã hủy":
                    filtered = _allBookings.Where(b => b.TrangThai == "DaHuy");
                    break;
            }

            dgvBookings.DataSource = filtered.ToList();
            UpdateControlStates();
        }

        // ====================================================================
        // 3. XỬ LÝ CÁC HÀNH ĐỘNG DUYỆT / CHECK-IN / HỦY ĐƠN
        // ====================================================================

        // [XÁC NHẬN ĐƠN TỪ WEB] - Chuyển từ ChoXacNhan -> DaXacNhan
        private async void btnConfirm_Click(object sender, EventArgs e)
        {
            var selectedBooking = GetSelectedBooking();
            if (selectedBooking == null) return;

            if (selectedBooking.TrangThai != "ChoXacNhan")
            {
                MessageBox.Show("Chỉ có thể xác nhận các đơn đang ở trạng thái 'Chờ xác nhận'!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show($"Xác nhận duyệt đơn đặt phòng #{selectedBooking.Id} của khách hàng {selectedBooking.TenKhachHang}?",
                                         "Xác nhận đơn", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                await UpdateBookingStatusAsync(selectedBooking.Id, "DaXacNhan", "Xác nhận đơn thành công!");
            }
        }

        // [NHẬN PHÒNG - CHECK-IN] - Chuyển từ DaXacNhan -> DaNhanPhong
        private async void btnCheckIn_Click(object sender, EventArgs e)
        {
            var selectedBooking = GetSelectedBooking();
            if (selectedBooking == null) return;

            if (selectedBooking.TrangThai != "DaXacNhan")
            {
                MessageBox.Show("Chỉ có thể làm thủ tục Check-in cho đơn đã được xác nhận!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show($"Tiến hành Check-in nhận phòng cho khách {selectedBooking.TenKhachHang}?",
                                         "Thực hiện Check-in", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                await UpdateBookingStatusAsync(selectedBooking.Id, "DaNhanPhong", "Khách đã nhận phòng thành công!");
            }
        }

        // [HỦY ĐƠN ĐẶT PHÒNG] - Chuyển sang DaHuy
        private async void btnCancel_Click(object sender, EventArgs e)
        {
            var selectedBooking = GetSelectedBooking();
            if (selectedBooking == null) return;

            if (selectedBooking.TrangThai == "DaThanhToan" || selectedBooking.TrangThai == "DaHuy")
            {
                MessageBox.Show("Không thể hủy đơn đặt phòng này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show($"Bạn có chắc chắn muốn HỦY đơn đặt phòng #{selectedBooking.Id} không?",
                                         "Xác nhận hủy", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                await UpdateBookingStatusAsync(selectedBooking.Id, "DaHuy", "Đã hủy đơn đặt phòng!");
            }
        }

        // Hàm dùng chung để cập nhật trạng thái đơn qua API
        private async Task UpdateBookingStatusAsync(int bookingId, string newStatus, string successMsg)
        {
            try
            {
                using (HttpClient client = BaseApiClient.GetClient())
                {
                    var payload = new { TrangThai = newStatus };
                    HttpResponseMessage response = await client.PutAsJsonAsync($"api/DonDatPhong/{bookingId}/trang-thai", payload);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show(successMsg, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        await LoadBookingsAsync();
                    }
                    else
                    {
                        MessageBox.Show($"Cập nhật thất bại. Mã lỗi API: {response.StatusCode}", "Lỗi API", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kết nối khi cập nhật đơn: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ====================================================================
        // 4. SỰ KIỆN PHỤ VÀ HELPER
        // ====================================================================
        private BookingViewModel? GetSelectedBooking()
        {
            if (dgvBookings.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một đơn đặt phòng từ danh sách!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }

            return dgvBookings.SelectedRows[0].DataBoundItem as BookingViewModel;
        }

        private void dgvBookings_SelectionChanged(object sender, EventArgs e)
        {
            UpdateControlStates();
        }

        private void UpdateControlStates()
        {
            var booking = dgvBookings.SelectedRows.Count > 0
                ? dgvBookings.SelectedRows[0].DataBoundItem as BookingViewModel
                : null;

            if (booking == null)
            {
                btnConfirm.Enabled = false;
                btnCheckIn.Enabled = false;
                btnCheckOut.Enabled = false;
                btnCancel.Enabled = false;
                return;
            }

            btnConfirm.Enabled = (booking.TrangThai == "ChoXacNhan");
            btnCheckIn.Enabled = (booking.TrangThai == "DaXacNhan");
            btnCheckOut.Enabled = (booking.TrangThai == "DaNhanPhong"); // Bật nút Trả phòng khi khách đang ở
            btnCancel.Enabled = (booking.TrangThai == "ChoXacNhan" || booking.TrangThai == "DaXacNhan");
        }

        private void cboStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await LoadBookingsAsync();
        }

        // Sự kiện bấm nút "+ Đặt tại quầy"
        private async void btnCreateAtCounter_Click(object sender, EventArgs e)
        {
            FrmCreateBooking frm = new FrmCreateBooking();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                // Tự động làm mới danh sách đơn sau khi lưu thành công
                await LoadBookingsAsync();
            }
        }

        // [TRẢ PHÒNG - CHECK-OUT] - Chuyển từ DaNhanPhong -> DaTraPhong & Cập nhật trạng thái Phòng về Trong
        private async void btnCheckOut_Click(object sender, EventArgs e)
        {
            var selectedBooking = GetSelectedBooking();
            if (selectedBooking == null) return;

            if (selectedBooking.TrangThai != "DaNhanPhong")
            {
                MessageBox.Show("Chỉ có thể làm thủ tục Trả phòng cho các đơn đang ở trạng thái 'Đã nhận phòng'!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show($"Xác nhận khách hàng {selectedBooking.TenKhachHang} trả phòng #{selectedBooking.SoPhong}?",
                                         "Thực hiện Check-out", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                // 1. Cập nhật trạng thái đơn đặt phòng thành 'DaTraPhong' (hoặc 'DaThanhToan')
                await UpdateBookingStatusAsync(selectedBooking.Id, "DaTraPhong", "Trả phòng thành công!");

                // 2. Đồng thời gọi API giải phóng phòng về trạng thái 'Trong'
                if (!string.IsNullOrEmpty(selectedBooking.SoPhong))
                {
                    await UpdateRoomStatusAsync(selectedBooking.SoPhong, "Trong");
                }
            }
        }

        // Hàm hỗ trợ gọi API cập nhật trạng thái phòng khi Check-in / Check-out
        private async Task UpdateRoomStatusAsync(string soPhong, string roomStatus)
        {
            try
            {
                using (HttpClient client = BaseApiClient.GetClient())
                {
                    var payload = new { TrangThai = roomStatus };
                    await client.PutAsJsonAsync($"api/Phong/{soPhong}/trang-thai", payload);
                }
            }
            catch
            {
                // Xử lý im lặng hoặc log lỗi nếu cần
            }
        }
    }
}