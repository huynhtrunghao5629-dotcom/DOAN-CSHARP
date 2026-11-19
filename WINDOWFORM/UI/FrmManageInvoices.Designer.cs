namespace HotelManager.UI
{
    partial class FrmManageInvoices
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.grpSelect = new System.Windows.Forms.GroupBox();
            this.lblSelectBooking = new System.Windows.Forms.Label();
            this.cboOccupiedBookings = new System.Windows.Forms.ComboBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.grpDetail = new System.Windows.Forms.GroupBox();
            this.lblCustomerName = new System.Windows.Forms.Label();
            this.txtCustomerName = new System.Windows.Forms.TextBox();
            this.lblRoomNumber = new System.Windows.Forms.Label();
            this.txtRoomNumber = new System.Windows.Forms.TextBox();
            this.lblCheckIn = new System.Windows.Forms.Label();
            this.txtCheckIn = new System.Windows.Forms.TextBox();
            this.lblCheckOut = new System.Windows.Forms.Label();
            this.dtpCheckOut = new System.Windows.Forms.DateTimePicker();
            this.lblDays = new System.Windows.Forms.Label();
            this.txtDays = new System.Windows.Forms.TextBox();
            this.lblRoomRate = new System.Windows.Forms.Label();
            this.txtRoomRate = new System.Windows.Forms.TextBox();
            this.lblRoomTotal = new System.Windows.Forms.Label();
            this.txtRoomTotal = new System.Windows.Forms.TextBox();
            this.lblServiceTotal = new System.Windows.Forms.Label();
            this.txtServiceTotal = new System.Windows.Forms.TextBox();
            this.lblGrandTotal = new System.Windows.Forms.Label();
            this.lblGrandTotalAmount = new System.Windows.Forms.Label();
            this.dgvServices = new System.Windows.Forms.DataGridView();
            this.lblServicesList = new System.Windows.Forms.Label();
            this.btnPay = new System.Windows.Forms.Button();
            this.btnPrintInvoice = new System.Windows.Forms.Button();
            this.grpSelect.SuspendLayout();
            this.grpDetail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvServices)).BeginInit();
            this.SuspendLayout();
            // 
            // grpSelect
            // 
            this.grpSelect.Controls.Add(this.btnRefresh);
            this.grpSelect.Controls.Add(this.cboOccupiedBookings);
            this.grpSelect.Controls.Add(this.lblSelectBooking);
            this.grpSelect.Location = new System.Drawing.Point(12, 12);
            this.grpSelect.Name = "grpSelect";
            this.grpSelect.Size = new System.Drawing.Size(860, 55);
            this.grpSelect.TabIndex = 0;
            this.grpSelect.TabStop = false;
            this.grpSelect.Text = "Chọn đơn đặt phòng cần thanh toán";
            // 
            // lblSelectBooking
            // 
            this.lblSelectBooking.AutoSize = true;
            this.lblSelectBooking.Location = new System.Drawing.Point(20, 23);
            this.lblSelectBooking.Name = "lblSelectBooking";
            this.lblSelectBooking.Size = new System.Drawing.Size(130, 15);
            this.lblSelectBooking.Text = "Khách đang ở (Phòng):";
            // 
            // cboOccupiedBookings
            // 
            this.cboOccupiedBookings.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboOccupiedBookings.FormattingEnabled = true;
            this.cboOccupiedBookings.Location = new System.Drawing.Point(160, 20);
            this.cboOccupiedBookings.Name = "cboOccupiedBookings";
            this.cboOccupiedBookings.Size = new System.Drawing.Size(350, 23);
            this.cboOccupiedBookings.TabIndex = 1;
            this.cboOccupiedBookings.SelectedIndexChanged += new System.EventHandler(this.cboOccupiedBookings_SelectedIndexChanged);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(530, 18);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(100, 26);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.Text = "Tải lại danh sách";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // grpDetail
            // 
            this.grpDetail.Controls.Add(this.dtpCheckOut);
            this.grpDetail.Controls.Add(this.txtServiceTotal);
            this.grpDetail.Controls.Add(this.lblServiceTotal);
            this.grpDetail.Controls.Add(this.txtRoomTotal);
            this.grpDetail.Controls.Add(this.lblRoomTotal);
            this.grpDetail.Controls.Add(this.txtRoomRate);
            this.grpDetail.Controls.Add(this.lblRoomRate);
            this.grpDetail.Controls.Add(this.txtDays);
            this.grpDetail.Controls.Add(this.lblDays);
            this.grpDetail.Controls.Add(this.lblCheckOut);
            this.grpDetail.Controls.Add(this.txtCheckIn);
            this.grpDetail.Controls.Add(this.lblCheckIn);
            this.grpDetail.Controls.Add(this.txtRoomNumber);
            this.grpDetail.Controls.Add(this.lblRoomNumber);
            this.grpDetail.Controls.Add(this.txtCustomerName);
            this.grpDetail.Controls.Add(this.lblCustomerName);
            this.grpDetail.Location = new System.Drawing.Point(12, 73);
            this.grpDetail.Name = "grpDetail";
            this.grpDetail.Size = new System.Drawing.Size(860, 125);
            this.grpDetail.TabIndex = 1;
            this.grpDetail.TabStop = false;
            this.grpDetail.Text = "Chi tiết tính tiền";
            // 
            // lblCustomerName
            // 
            this.lblCustomerName.AutoSize = true;
            this.lblCustomerName.Location = new System.Drawing.Point(20, 25);
            this.lblCustomerName.Name = "lblCustomerName";
            this.lblCustomerName.Size = new System.Drawing.Size(73, 15);
            this.lblCustomerName.Text = "Khách hàng:";
            // 
            // txtCustomerName
            // 
            this.txtCustomerName.Location = new System.Drawing.Point(100, 22);
            this.txtCustomerName.Name = "txtCustomerName";
            this.txtCustomerName.ReadOnly = true;
            this.txtCustomerName.Size = new System.Drawing.Size(180, 23);
            this.txtCustomerName.TabIndex = 1;
            // 
            // lblRoomNumber
            // 
            this.lblRoomNumber.AutoSize = true;
            this.lblRoomNumber.Location = new System.Drawing.Point(310, 25);
            this.lblRoomNumber.Name = "lblRoomNumber";
            this.lblRoomNumber.Size = new System.Drawing.Size(61, 15);
            this.lblRoomNumber.Text = "Số phòng:";
            // 
            // txtRoomNumber
            // 
            this.txtRoomNumber.Location = new System.Drawing.Point(380, 22);
            this.txtRoomNumber.Name = "txtRoomNumber";
            this.txtRoomNumber.ReadOnly = true;
            this.txtRoomNumber.Size = new System.Drawing.Size(130, 23);
            this.txtRoomNumber.TabIndex = 3;
            // 
            // lblCheckIn
            // 
            this.lblCheckIn.AutoSize = true;
            this.lblCheckIn.Location = new System.Drawing.Point(540, 25);
            this.lblCheckIn.Name = "lblCheckIn";
            this.lblCheckIn.Size = new System.Drawing.Size(102, 15);
            this.lblCheckIn.Text = "Ngày nhận phòng:";
            // 
            // txtCheckIn
            // 
            this.txtCheckIn.Location = new System.Drawing.Point(650, 22);
            this.txtCheckIn.Name = "txtCheckIn";
            this.txtCheckIn.ReadOnly = true;
            this.txtCheckIn.Size = new System.Drawing.Size(180, 23);
            this.txtCheckIn.TabIndex = 5;
            // 
            // lblCheckOut
            // 
            this.lblCheckOut.AutoSize = true;
            this.lblCheckOut.Location = new System.Drawing.Point(540, 58);
            this.lblCheckOut.Name = "lblCheckOut";
            this.lblCheckOut.Size = new System.Drawing.Size(92, 15);
            this.lblCheckOut.Text = "Ngày trả phòng:";
            // 
            // dtpCheckOut
            // 
            this.dtpCheckOut.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpCheckOut.Location = new System.Drawing.Point(650, 55);
            this.dtpCheckOut.Name = "dtpCheckOut";
            this.dtpCheckOut.Size = new System.Drawing.Size(180, 23);
            this.dtpCheckOut.TabIndex = 7;
            this.dtpCheckOut.ValueChanged += new System.EventHandler(this.dtpCheckOut_ValueChanged);
            // 
            // lblDays
            // 
            this.lblDays.AutoSize = true;
            this.lblDays.Location = new System.Drawing.Point(20, 58);
            this.lblDays.Name = "lblDays";
            this.lblDays.Size = new System.Drawing.Size(61, 15);
            this.lblDays.Text = "Số ngày ở:";
            // 
            // txtDays
            // 
            this.txtDays.Location = new System.Drawing.Point(100, 55);
            this.txtDays.Name = "txtDays";
            this.txtDays.ReadOnly = true;
            this.txtDays.Size = new System.Drawing.Size(180, 23);
            this.txtDays.TabIndex = 9;
            // 
            // lblRoomRate
            // 
            this.lblRoomRate.AutoSize = true;
            this.lblRoomRate.Location = new System.Drawing.Point(310, 58);
            this.lblRoomRate.Name = "lblRoomRate";
            this.lblRoomRate.Size = new System.Drawing.Size(65, 15);
            this.lblRoomRate.Text = "Giá phòng:";
            // 
            // txtRoomRate
            // 
            this.txtRoomRate.Location = new System.Drawing.Point(380, 55);
            this.txtRoomRate.Name = "txtRoomRate";
            this.txtRoomRate.ReadOnly = true;
            this.txtRoomRate.Size = new System.Drawing.Size(130, 23);
            this.txtRoomRate.TabIndex = 11;
            // 
            // lblRoomTotal
            // 
            this.lblRoomTotal.AutoSize = true;
            this.lblRoomTotal.Location = new System.Drawing.Point(20, 91);
            this.lblRoomTotal.Name = "lblRoomTotal";
            this.lblRoomTotal.Size = new System.Drawing.Size(72, 15);
            this.lblRoomTotal.Text = "Tiền phòng:";
            // 
            // txtRoomTotal
            // 
            this.txtRoomTotal.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.txtRoomTotal.Location = new System.Drawing.Point(100, 88);
            this.txtRoomTotal.Name = "txtRoomTotal";
            this.txtRoomTotal.ReadOnly = true;
            this.txtRoomTotal.Size = new System.Drawing.Size(180, 23);
            this.txtRoomTotal.TabIndex = 13;
            // 
            // lblServiceTotal
            // 
            this.lblServiceTotal.AutoSize = true;
            this.lblServiceTotal.Location = new System.Drawing.Point(310, 91);
            this.lblServiceTotal.Name = "lblServiceTotal";
            this.lblServiceTotal.Size = new System.Drawing.Size(73, 15);
            this.lblServiceTotal.Text = "Tiền dịch vụ:";
            // 
            // txtServiceTotal
            // 
            this.txtServiceTotal.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.txtServiceTotal.Location = new System.Drawing.Point(380, 88);
            this.txtServiceTotal.Name = "txtServiceTotal";
            this.txtServiceTotal.ReadOnly = true;
            this.txtServiceTotal.Size = new System.Drawing.Size(130, 23);
            this.txtServiceTotal.TabIndex = 15;
            // 
            // lblServicesList
            // 
            this.lblServicesList.AutoSize = true;
            this.lblServicesList.Location = new System.Drawing.Point(12, 208);
            this.lblServicesList.Name = "lblServicesList";
            this.lblServicesList.Size = new System.Drawing.Size(193, 15);
            this.lblServicesList.Text = "Danh sách dịch vụ phòng đã sử dụng:";
            // 
            // dgvServices
            // 
            this.dgvServices.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvServices.Location = new System.Drawing.Point(12, 228);
            this.dgvServices.Name = "dgvServices";
            this.dgvServices.Size = new System.Drawing.Size(860, 210);
            this.dgvServices.TabIndex = 2;
            // 
            // lblGrandTotal
            // 
            this.lblGrandTotal.AutoSize = true;
            this.lblGrandTotal.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblGrandTotal.Location = new System.Drawing.Point(400, 456);
            this.lblGrandTotal.Name = "lblGrandTotal";
            this.lblGrandTotal.Size = new System.Drawing.Size(168, 21);
            this.lblGrandTotal.Text = "TỔNG THANH TOÁN:";
            // 
            // lblGrandTotalAmount
            // 
            this.lblGrandTotalAmount.AutoSize = true;
            this.lblGrandTotalAmount.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblGrandTotalAmount.ForeColor = System.Drawing.Color.Red;
            this.lblGrandTotalAmount.Location = new System.Drawing.Point(575, 453);
            this.lblGrandTotalAmount.Name = "lblGrandTotalAmount";
            this.lblGrandTotalAmount.Size = new System.Drawing.Size(70, 25);
            this.lblGrandTotalAmount.Text = "0 VNĐ";
            // 
            // btnPay
            // 
            this.btnPay.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.btnPay.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnPay.ForeColor = System.Drawing.Color.White;
            this.btnPay.Location = new System.Drawing.Point(730, 450);
            this.btnPay.Name = "btnPay";
            this.btnPay.Size = new System.Drawing.Size(140, 35);
            this.btnPay.TabIndex = 3;
            this.btnPay.Text = "Thanh toán & Trả phòng";
            this.btnPay.UseVisualStyleBackColor = false;
            this.btnPay.Click += new System.EventHandler(this.btnPay_Click);
            // 
            // btnPrintInvoice
            // 
            this.btnPrintInvoice.Location = new System.Drawing.Point(12, 452);
            this.btnPrintInvoice.Name = "btnPrintInvoice";
            this.btnPrintInvoice.Size = new System.Drawing.Size(120, 33);
            this.btnPrintInvoice.TabIndex = 4;
            this.btnPrintInvoice.Text = "Xem / In hóa đơn";
            this.btnPrintInvoice.UseVisualStyleBackColor = true;
            this.btnPrintInvoice.Click += new System.EventHandler(this.btnPrintInvoice_Click);
            // 
            // FrmManageInvoices
            // 
            this.ClientSize = new System.Drawing.Size(884, 502);
            this.Controls.Add(this.btnPrintInvoice);
            this.Controls.Add(this.btnPay);
            this.Controls.Add(this.lblGrandTotalAmount);
            this.Controls.Add(this.lblGrandTotal);
            this.Controls.Add(this.dgvServices);
            this.Controls.Add(this.lblServicesList);
            this.Controls.Add(this.grpDetail);
            this.Controls.Add(this.grpSelect);
            this.Name = "FrmManageInvoices";
            this.Text = "Thanh toán & Xuất hóa đơn";
            this.Load += new System.EventHandler(this.FrmManageInvoices_Load);
            this.grpSelect.ResumeLayout(false);
            this.grpSelect.PerformLayout();
            this.grpDetail.ResumeLayout(false);
            this.grpDetail.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvServices)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.GroupBox grpSelect;
        private System.Windows.Forms.Label lblSelectBooking;
        private System.Windows.Forms.ComboBox cboOccupiedBookings;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.GroupBox grpDetail;
        private System.Windows.Forms.Label lblCustomerName;
        private System.Windows.Forms.TextBox txtCustomerName;
        private System.Windows.Forms.Label lblRoomNumber;
        private System.Windows.Forms.TextBox txtRoomNumber;
        private System.Windows.Forms.Label lblCheckIn;
        private System.Windows.Forms.TextBox txtCheckIn;
        private System.Windows.Forms.Label lblCheckOut;
        private System.Windows.Forms.DateTimePicker dtpCheckOut;
        private System.Windows.Forms.Label lblDays;
        private System.Windows.Forms.TextBox txtDays;
        private System.Windows.Forms.Label lblRoomRate;
        private System.Windows.Forms.TextBox txtRoomRate;
        private System.Windows.Forms.Label lblRoomTotal;
        private System.Windows.Forms.TextBox txtRoomTotal;
        private System.Windows.Forms.Label lblServiceTotal;
        private System.Windows.Forms.TextBox txtServiceTotal;
        private System.Windows.Forms.Label lblServicesList;
        private System.Windows.Forms.DataGridView dgvServices;
        private System.Windows.Forms.Label lblGrandTotal;
        private System.Windows.Forms.Label lblGrandTotalAmount;
        private System.Windows.Forms.Button btnPay;
        private System.Windows.Forms.Button btnPrintInvoice;
    }
}