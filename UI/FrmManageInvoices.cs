using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using HotelManagement.Desktop.Models;
using HotelManagement.Desktop.Services;

namespace HotelManagement.Desktop.UI
{
    public partial class FrmManageInvoices : Form
    {
        private readonly InvoiceService _invoiceService;
        private readonly BookingService _bookingService;
        private readonly UserSession _currentUser;

        private ComboBox cboBookingId;
        private TextBox txtCustomerName;
        private TextBox txtRoomNumber;
        private TextBox txtTotalAmount;
        private DateTimePicker dtpPaymentDate;
        private ComboBox cboPaymentMethod;
        private ComboBox cboStatus;
        private DataGridView dgvInvoices;

        private Button btnPay;
        private Button btnReset;
        private Button btnDelete;

        private int _selectedInvoiceId = 0;

        public FrmManageInvoices()
        {
            InitializeComponentCustom();
            _invoiceService = new InvoiceService();
            _bookingService = new BookingService();
        }

        public FrmManageInvoices(UserSession userSession) : this()
        {
            _currentUser = userSession;
        }

        private void InitializeComponentCustom()
        {
            this.SuspendLayout();

            Label lblBookingId = new Label() { Text = "Mã đặt phòng:", Location = new System.Drawing.Point(30, 30), AutoSize = true };
            cboBookingId = new ComboBox() { Location = new System.Drawing.Point(130, 27), Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
            cboBookingId.SelectedIndexChanged += CboBookingId_SelectedIndexChanged;

            Label lblCustomerName = new Label() { Text = "Khách hàng:", Location = new System.Drawing.Point(30, 70), AutoSize = true };
            txtCustomerName = new TextBox() { Location = new System.Drawing.Point(130, 67), Width = 180, ReadOnly = true };

            Label lblRoomNumber = new Label() { Text = "Số phòng:", Location = new System.Drawing.Point(30, 110), AutoSize = true };
            txtRoomNumber = new TextBox() { Location = new System.Drawing.Point(130, 107), Width = 180, ReadOnly = true };

            Label lblTotalAmount = new Label() { Text = "Tổng tiền:", Location = new System.Drawing.Point(30, 150), AutoSize = true };
            txtTotalAmount = new TextBox() { Location = new System.Drawing.Point(130, 147), Width = 180, ReadOnly = true };

            Label lblPaymentDate = new Label() { Text = "Ngày thanh toán:", Location = new System.Drawing.Point(340, 30), AutoSize = true };
            dtpPaymentDate = new DateTimePicker() { Location = new System.Drawing.Point(440, 27), Width = 180, Format = DateTimePickerFormat.Short };

            Label lblPaymentMethod = new Label() { Text = "Phương thức:", Location = new System.Drawing.Point(340, 70), AutoSize = true };
            cboPaymentMethod = new ComboBox() { Location = new System.Drawing.Point(440, 67), Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
            cboPaymentMethod.Items.AddRange(new object[] { "Tiền mặt", "Chuyển khoản", "Thẻ tín dụng" });
            cboPaymentMethod.SelectedIndex = 0;

            Label lblStatus = new Label() { Text = "Trạng thái:", Location = new System.Drawing.Point(340, 110), AutoSize = true };
            cboStatus = new ComboBox() { Location = new System.Drawing.Point(440, 107), Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
            cboStatus.Items.AddRange(new object[] { "Chưa thanh toán", "Đã thanh toán" });
            cboStatus.SelectedIndex = 1;

            btnReset = new Button() { Text = "Làm mới", Location = new System.Drawing.Point(440, 150), Width = 80, Height = 30 };
            btnReset.Click += BtnReset_Click;

            btnPay = new Button() { Text = "Thanh toán", Location = new System.Drawing.Point(540, 150), Width = 80, Height = 30 };
            btnPay.Click += BtnPay_Click;

            btnDelete = new Button() { Text = "Xóa hóa đơn", Location = new System.Drawing.Point(440, 190), Width = 180, Height = 30 };
            btnDelete.Click += BtnDelete_Click;

            dgvInvoices = new DataGridView()
            {
                Location = new System.Drawing.Point(30, 240),
                Width = 720,
                Height = 200,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            dgvInvoices.CellClick += DgvInvoices_CellClick;

            this.ClientSize = new System.Drawing.Size(780, 470);
            this.Controls.Add(lblBookingId);
            this.Controls.Add(cboBookingId);
            this.Controls.Add(lblCustomerName);
            this.Controls.Add(txtCustomerName);
            this.Controls.Add(lblRoomNumber);
            this.Controls.Add(txtRoomNumber);
            this.Controls.Add(lblTotalAmount);
            this.Controls.Add(txtTotalAmount);
            this.Controls.Add(lblPaymentDate);
            this.Controls.Add(dtpPaymentDate);
            this.Controls.Add(lblPaymentMethod);
            this.Controls.Add(cboPaymentMethod);
            this.Controls.Add(lblStatus);
            this.Controls.Add(cboStatus);
            this.Controls.Add(btnReset);
            this.Controls.Add(btnPay);
            this.Controls.Add(btnDelete);
            this.Controls.Add(dgvInvoices);

            this.Name = "FrmManageInvoices";
            this.Text = "Quản lý Hóa đơn & Thanh toán";
            this.Load += FrmManageInvoices_Load;

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private async void FrmManageInvoices_Load(object sender, EventArgs e)
        {
            await LoadBookingsAsync();
            await LoadInvoicesAsync();
        }

        private async Task LoadBookingsAsync()
        {
            var bookings = await _bookingService.GetAllBookingsAsync();
            cboBookingId.DataSource = null;
            cboBookingId.DataSource = bookings;
            cboBookingId.DisplayMember = "BookingId";
            cboBookingId.ValueMember = "BookingId";
            cboBookingId.SelectedIndex = -1;
            ClearBookingFields();
        }

        private async Task LoadInvoicesAsync()
        {
            var invoices = await _invoiceService.GetAllInvoicesAsync();
            dgvInvoices.DataSource = null;
            dgvInvoices.DataSource = invoices;
        }

        private void CboBookingId_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboBookingId.SelectedItem is BookingDto booking)
            {
                txtCustomerName.Text = booking.CustomerName;
                txtRoomNumber.Text = booking.RoomNumber;
                txtTotalAmount.Text = booking.TotalPrice.ToString();
            }
        }

        private async void BtnPay_Click(object sender, EventArgs e)
        {
            if (cboBookingId.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn Mã đặt phòng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var booking = (BookingDto)cboBookingId.SelectedItem;

            var invoice = new InvoiceDto
            {
                InvoiceId = _selectedInvoiceId,
                BookingId = booking.BookingId,
                CustomerName = booking.CustomerName,
                RoomNumber = booking.RoomNumber,
                TotalAmount = booking.TotalPrice,
                PaymentDate = dtpPaymentDate.Value,
                PaymentMethod = cboPaymentMethod.SelectedItem.ToString(),
                Status = cboStatus.SelectedItem.ToString()
            };

            await _invoiceService.SaveInvoiceAsync(invoice);
            MessageBox.Show("Lưu hóa đơn thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

            await LoadInvoicesAsync();
            ResetForm();
        }

        private async void BtnReset_Click(object sender, EventArgs e)
        {
            await LoadBookingsAsync();
            ResetForm();
        }

        private async void BtnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedInvoiceId == 0)
            {
                MessageBox.Show("Vui lòng chọn hóa đơn cần xóa từ danh sách!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Bạn có chắc chắn muốn xóa hóa đơn này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                await _invoiceService.DeleteInvoiceAsync(_selectedInvoiceId);
                await LoadInvoicesAsync();
                ResetForm();
            }
        }

        private void DgvInvoices_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvInvoices.Rows[e.RowIndex].DataBoundItem is InvoiceDto invoice)
            {
                _selectedInvoiceId = invoice.InvoiceId;
                cboBookingId.SelectedValue = invoice.BookingId;
                txtCustomerName.Text = invoice.CustomerName;
                txtRoomNumber.Text = invoice.RoomNumber;
                txtTotalAmount.Text = invoice.TotalAmount.ToString();
                dtpPaymentDate.Value = invoice.PaymentDate;
                cboPaymentMethod.SelectedItem = invoice.PaymentMethod;
                cboStatus.SelectedItem = invoice.Status;
            }
        }

        private void ResetForm()
        {
            _selectedInvoiceId = 0;
            cboBookingId.SelectedIndex = -1;
            ClearBookingFields();
            cboPaymentMethod.SelectedIndex = 0;
            cboStatus.SelectedIndex = 1;
        }

        private void ClearBookingFields()
        {
            txtCustomerName.Clear();
            txtRoomNumber.Clear();
            txtTotalAmount.Clear();
        }
    }
}