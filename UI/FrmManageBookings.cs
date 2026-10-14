using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using HotelManagement.Desktop.Models;
using HotelManagement.Desktop.Services;

namespace HotelManagement.Desktop.UI
{
    public partial class FrmManageBookings : Form
    {
        private readonly BookingService _bookingService;
        private readonly CustomerService _customerService;
        private readonly RoomService _roomService;
        private int _selectedBookingId = 0;
        private readonly UserSession _currentUser;

        private DataGridView dgvBookings;
        private ComboBox cbCustomer;
        private ComboBox cbRoom;
        private DateTimePicker dtpCheckIn;
        private DateTimePicker dtpCheckOut;
        private TextBox txtTotalPrice;
        private ComboBox cbStatus;
        private Button btnSave;
        private Button btnDelete;
        private Button btnReset;

        public FrmManageBookings()
        {
            InitializeComponentCustom();
            _bookingService = new BookingService();
            _customerService = new CustomerService();
            _roomService = new RoomService();
        }

        public FrmManageBookings(UserSession userSession) : this()
        {
            _currentUser = userSession;
        }

        private void InitializeComponentCustom()
        {
            this.SuspendLayout();

            dgvBookings = new DataGridView();
            cbCustomer = new ComboBox();
            cbRoom = new ComboBox();
            dtpCheckIn = new DateTimePicker();
            dtpCheckOut = new DateTimePicker();
            txtTotalPrice = new TextBox();
            cbStatus = new ComboBox();
            btnSave = new Button();
            btnDelete = new Button();
            btnReset = new Button();

            ((System.ComponentModel.ISupportInitialize)(dgvBookings)).BeginInit();

            // dgvBookings
            dgvBookings.Dock = DockStyle.Bottom;
            dgvBookings.Height = 220;
            dgvBookings.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvBookings.CellClick += DgvBookings_CellClick;

            // Controls properties
            cbCustomer.Location = new System.Drawing.Point(150, 30);
            cbCustomer.Width = 200;
            cbCustomer.DropDownStyle = ComboBoxStyle.DropDownList;

            cbRoom.Location = new System.Drawing.Point(150, 80);
            cbRoom.Width = 200;
            cbRoom.DropDownStyle = ComboBoxStyle.DropDownList;

            // Thiết lập giá trị mặc định và sự kiện cho ngày nhận phòng
            dtpCheckIn.Location = new System.Drawing.Point(150, 130);
            dtpCheckIn.Format = DateTimePickerFormat.Short;
            dtpCheckIn.Value = DateTime.Today;
            dtpCheckIn.ValueChanged += DtpCheckIn_ValueChanged;

            // Thiết lập giá trị mặc định ngày trả phòng (lớn hơn ngày nhận 1 ngày)
            dtpCheckOut.Location = new System.Drawing.Point(150, 180);
            dtpCheckOut.Format = DateTimePickerFormat.Short;
            dtpCheckOut.Value = DateTime.Today.AddDays(1);

            txtTotalPrice.Location = new System.Drawing.Point(500, 30);
            txtTotalPrice.Width = 200;

            cbStatus.Location = new System.Drawing.Point(500, 80);
            cbStatus.Width = 200;
            cbStatus.Items.AddRange(new object[] { "Đang đặt", "Đã nhận phòng", "Đã trả phòng", "Đã hủy" });

            // Buttons
            btnReset.Text = "Làm mới";
            btnReset.Location = new System.Drawing.Point(500, 130);
            btnReset.Width = 90;
            btnReset.Click += BtnReset_Click;

            btnSave.Text = "Lưu";
            btnSave.Location = new System.Drawing.Point(600, 130);
            btnSave.Width = 100;
            btnSave.Click += BtnSave_Click;

            btnDelete.Text = "Xóa";
            btnDelete.Location = new System.Drawing.Point(500, 175);
            btnDelete.Width = 200;
            btnDelete.Click += BtnDelete_Click;

            // Labels
            Label lbl1 = new Label() { Text = "Khách hàng:", Location = new System.Drawing.Point(40, 33) };
            Label lbl2 = new Label() { Text = "Phòng:", Location = new System.Drawing.Point(40, 83) };
            Label lbl3 = new Label() { Text = "Ngày nhận:", Location = new System.Drawing.Point(40, 133) };
            Label lbl4 = new Label() { Text = "Ngày trả:", Location = new System.Drawing.Point(40, 183) };
            Label lbl5 = new Label() { Text = "Tổng tiền:", Location = new System.Drawing.Point(400, 33) };
            Label lbl6 = new Label() { Text = "Trạng thái:", Location = new System.Drawing.Point(400, 83) };

            // Form
            this.ClientSize = new System.Drawing.Size(780, 480);
            this.Controls.Add(dgvBookings);
            this.Controls.Add(cbCustomer);
            this.Controls.Add(cbRoom);
            this.Controls.Add(dtpCheckIn);
            this.Controls.Add(dtpCheckOut);
            this.Controls.Add(txtTotalPrice);
            this.Controls.Add(cbStatus);
            this.Controls.Add(btnReset);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnDelete);
            this.Controls.Add(lbl1);
            this.Controls.Add(lbl2);
            this.Controls.Add(lbl3);
            this.Controls.Add(lbl4);
            this.Controls.Add(lbl5);
            this.Controls.Add(lbl6);
            this.Name = "FrmManageBookings";
            this.Text = "Quản lý Đặt phòng";
            this.Load += FrmManageBookings_Load;

            ((System.ComponentModel.ISupportInitialize)(dgvBookings)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void DtpCheckIn_ValueChanged(object sender, EventArgs e)
        {
            // Tự động đẩy ngày trả phòng lên 1 ngày nếu ngày nhận thay đổi vượt quá ngày trả
            if (dtpCheckOut.Value <= dtpCheckIn.Value)
            {
                dtpCheckOut.Value = dtpCheckIn.Value.AddDays(1);
            }
        }

        private async void FrmManageBookings_Load(object sender, EventArgs e)
        {
            await LoadComboboxDataAsync();
            await LoadBookingDataAsync();
        }

        private async Task LoadComboboxDataAsync()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            cbCustomer.DataSource = customers;
            cbCustomer.DisplayMember = "FullName";
            cbCustomer.ValueMember = "CustomerId";

            var rooms = await _roomService.GetAllRoomsAsync();
            cbRoom.DataSource = rooms;
            cbRoom.DisplayMember = "RoomNumber";
            cbRoom.ValueMember = "RoomId";
        }

        private async Task LoadBookingDataAsync()
        {
            var bookings = await _bookingService.GetAllBookingsAsync();
            dgvBookings.DataSource = null;
            dgvBookings.DataSource = bookings;
        }

        private void DgvBookings_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dgvBookings.Rows[e.RowIndex];
                _selectedBookingId = Convert.ToInt32(row.Cells["BookingId"].Value);
                cbCustomer.SelectedValue = row.Cells["CustomerId"].Value;
                cbRoom.SelectedValue = row.Cells["RoomId"].Value;
                dtpCheckIn.Value = Convert.ToDateTime(row.Cells["CheckInDate"].Value);
                dtpCheckOut.Value = Convert.ToDateTime(row.Cells["CheckOutDate"].Value);
                txtTotalPrice.Text = row.Cells["TotalPrice"].Value?.ToString();
                cbStatus.SelectedItem = row.Cells["Status"].Value?.ToString();
            }
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            _selectedBookingId = 0;
            txtTotalPrice.Clear();
            dtpCheckIn.Value = DateTime.Today;
            dtpCheckOut.Value = DateTime.Today.AddDays(1);
            if (cbStatus.Items.Count > 0) cbStatus.SelectedIndex = 0;
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra ngày trả phòng phải lớn hơn ngày nhận phòng
                if (dtpCheckOut.Value.Date <= dtpCheckIn.Value.Date)
                {
                    MessageBox.Show("Ngày trả phòng phải lớn hơn Ngày nhận phòng!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(txtTotalPrice.Text, out decimal totalPrice) || totalPrice < 0)
                {
                    MessageBox.Show("Vui lòng nhập tổng tiền hợp lệ!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var selectedCustomer = cbCustomer.SelectedItem as CustomerDto;
                var selectedRoom = cbRoom.SelectedItem as RoomDto;

                var booking = new BookingDto
                {
                    BookingId = _selectedBookingId,
                    CustomerId = selectedCustomer != null ? selectedCustomer.CustomerId : 1,
                    CustomerName = selectedCustomer != null ? selectedCustomer.FullName : "Khách lẻ",
                    RoomId = selectedRoom != null ? selectedRoom.RoomId : 1,
                    RoomNumber = selectedRoom != null ? selectedRoom.RoomNumber : "101",
                    CheckInDate = dtpCheckIn.Value,
                    CheckOutDate = dtpCheckOut.Value,
                    TotalPrice = totalPrice,
                    Status = cbStatus.SelectedItem?.ToString() ?? "Đang đặt"
                };

                await _bookingService.SaveBookingAsync(booking);

                // --- TỰ ĐỘNG CẬP NHẬT TRẠNG THÁI PHÒNG ---
                if (selectedRoom != null)
                {
                    string roomStatusToUpdate = (booking.Status == "Đã hủy" || booking.Status == "Đã trả phòng")
                                                ? "Available"
                                                : "Occupied";

                    await _roomService.UpdateRoomStatusAsync(selectedRoom.RoomNumber, roomStatusToUpdate);
                }
                // ------------------------------------------

                MessageBox.Show("Lưu thông tin đặt phòng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                BtnReset_Click(sender, e);
                await LoadBookingDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedBookingId == 0)
            {
                MessageBox.Show("Vui lòng chọn thông tin đặt phòng cần xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Bạn có chắc chắn muốn xóa?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                await _bookingService.DeleteBookingAsync(_selectedBookingId);
                MessageBox.Show("Đã xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                BtnReset_Click(sender, e);
                await LoadBookingDataAsync();
            }
        }
    }
}