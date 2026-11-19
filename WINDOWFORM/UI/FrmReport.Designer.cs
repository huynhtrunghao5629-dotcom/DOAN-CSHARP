namespace HotelManager.UI
{
    partial class FrmReport
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
            this.grpFilter = new System.Windows.Forms.GroupBox();
            this.lblMonth = new System.Windows.Forms.Label();
            this.cboMonth = new System.Windows.Forms.ComboBox();
            this.lblYear = new System.Windows.Forms.Label();
            this.nudYear = new System.Windows.Forms.NumericUpDown();
            this.btnViewReport = new System.Windows.Forms.Button();
            this.pnlSummary = new System.Windows.Forms.Panel();
            this.lblTotalRevenueTitle = new System.Windows.Forms.Label();
            this.lblTotalRevenue = new System.Windows.Forms.Label();
            this.lblTotalBookingsTitle = new System.Windows.Forms.Label();
            this.lblTotalBookings = new System.Windows.Forms.Label();
            this.lblOccupancyTitle = new System.Windows.Forms.Label();
            this.lblOccupancyRate = new System.Windows.Forms.Label();
            this.dgvReport = new System.Windows.Forms.DataGridView();
            this.grpFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudYear)).BeginInit();
            this.pnlSummary.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReport)).BeginInit();
            this.SuspendLayout();
            // 
            // grpFilter
            // 
            this.grpFilter.Controls.Add(this.btnViewReport);
            this.grpFilter.Controls.Add(this.nudYear);
            this.grpFilter.Controls.Add(this.lblYear);
            this.grpFilter.Controls.Add(this.cboMonth);
            this.grpFilter.Controls.Add(this.lblMonth);
            this.grpFilter.Location = new System.Drawing.Point(12, 12);
            this.grpFilter.Name = "grpFilter";
            this.grpFilter.Size = new System.Drawing.Size(860, 55);
            this.grpFilter.TabIndex = 0;
            this.grpFilter.TabStop = false;
            this.grpFilter.Text = "Thời gian báo cáo";
            // 
            // lblMonth
            // 
            this.lblMonth.AutoSize = true;
            this.lblMonth.Location = new System.Drawing.Point(20, 23);
            this.lblMonth.Name = "lblMonth";
            this.lblMonth.Size = new System.Drawing.Size(73, 15);
            this.lblMonth.Text = "Chọn tháng:";
            // 
            // cboMonth
            // 
            this.cboMonth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMonth.FormattingEnabled = true;
            this.cboMonth.Location = new System.Drawing.Point(100, 20);
            this.cboMonth.Name = "cboMonth";
            this.cboMonth.Size = new System.Drawing.Size(100, 23);
            this.cboMonth.TabIndex = 1;
            // 
            // lblYear
            // 
            this.lblYear.AutoSize = true;
            this.lblYear.Location = new System.Drawing.Point(230, 23);
            this.lblYear.Name = "lblYear";
            this.lblYear.Size = new System.Drawing.Size(66, 15);
            this.lblYear.Text = "Chọn năm:";
            // 
            // nudYear
            // 
            this.nudYear.Location = new System.Drawing.Point(300, 20);
            this.nudYear.Maximum = new decimal(new int[] { 2100, 0, 0, 0 });
            this.nudYear.Minimum = new decimal(new int[] { 2020, 0, 0, 0 });
            this.nudYear.Name = "nudYear";
            this.nudYear.Size = new System.Drawing.Size(100, 23);
            this.nudYear.TabIndex = 2;
            this.nudYear.Value = new decimal(new int[] { 2026, 0, 0, 0 });
            // 
            // btnViewReport
            // 
            this.btnViewReport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(123)))), ((int)(((byte)(255)))));
            this.btnViewReport.ForeColor = System.Drawing.Color.White;
            this.btnViewReport.Location = new System.Drawing.Point(430, 17);
            this.btnViewReport.Name = "btnViewReport";
            this.btnViewReport.Size = new System.Drawing.Size(130, 28);
            this.btnViewReport.TabIndex = 3;
            this.btnViewReport.Text = "Xem báo cáo";
            this.btnViewReport.UseVisualStyleBackColor = false;
            this.btnViewReport.Click += new System.EventHandler(this.btnViewReport_Click);
            // 
            // pnlSummary
            // 
            this.pnlSummary.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlSummary.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlSummary.Controls.Add(this.lblOccupancyRate);
            this.pnlSummary.Controls.Add(this.lblOccupancyTitle);
            this.pnlSummary.Controls.Add(this.lblTotalBookings);
            this.pnlSummary.Controls.Add(this.lblTotalBookingsTitle);
            this.pnlSummary.Controls.Add(this.lblTotalRevenue);
            this.pnlSummary.Controls.Add(this.lblTotalRevenueTitle);
            this.pnlSummary.Location = new System.Drawing.Point(12, 75);
            this.pnlSummary.Name = "pnlSummary";
            this.pnlSummary.Size = new System.Drawing.Size(860, 70);
            this.pnlSummary.TabIndex = 1;
            // 
            // lblTotalRevenueTitle
            // 
            this.lblTotalRevenueTitle.AutoSize = true;
            this.lblTotalRevenueTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.lblTotalRevenueTitle.Location = new System.Drawing.Point(20, 12);
            this.lblTotalRevenueTitle.Name = "lblTotalRevenueTitle";
            this.lblTotalRevenueTitle.Size = new System.Drawing.Size(128, 15);
            this.lblTotalRevenueTitle.Text = "TỔNG DOANH THU:";
            // 
            // lblTotalRevenue
            // 
            this.lblTotalRevenue.AutoSize = true;
            this.lblTotalRevenue.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTotalRevenue.ForeColor = System.Drawing.Color.Green;
            this.lblTotalRevenue.Location = new System.Drawing.Point(20, 35);
            this.lblTotalRevenue.Name = "lblTotalRevenue";
            this.lblTotalRevenue.Size = new System.Drawing.Size(58, 21);
            this.lblTotalRevenue.Text = "0 VNĐ";
            // 
            // lblTotalBookingsTitle
            // 
            this.lblTotalBookingsTitle.AutoSize = true;
            this.lblTotalBookingsTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.lblTotalBookingsTitle.Location = new System.Drawing.Point(320, 12);
            this.lblTotalBookingsTitle.Name = "lblTotalBookingsTitle";
            this.lblTotalBookingsTitle.Size = new System.Drawing.Size(130, 15);
            this.lblTotalBookingsTitle.Text = "TỔNG LƯỢT ĐẶT PHÒNG:";
            // 
            // lblTotalBookings
            // 
            this.lblTotalBookings.AutoSize = true;
            this.lblTotalBookings.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTotalBookings.ForeColor = System.Drawing.Color.Blue;
            this.lblTotalBookings.Location = new System.Drawing.Point(320, 35);
            this.lblTotalBookings.Name = "lblTotalBookings";
            this.lblTotalBookings.Size = new System.Drawing.Size(63, 21);
            this.lblTotalBookings.Text = "0 đơn";
            // 
            // lblOccupancyTitle
            // 
            this.lblOccupancyTitle.AutoSize = true;
            this.lblOccupancyTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.lblOccupancyTitle.Location = new System.Drawing.Point(600, 12);
            this.lblOccupancyTitle.Name = "lblOccupancyTitle";
            this.lblOccupancyTitle.Size = new System.Drawing.Size(158, 15);
            this.lblOccupancyTitle.Text = "CÔNG SUẤT PHÒNG TRUNG BÌNH:";
            // 
            // lblOccupancyRate
            // 
            this.lblOccupancyRate.AutoSize = true;
            this.lblOccupancyRate.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblOccupancyRate.ForeColor = System.Drawing.Color.DarkOrange;
            this.lblOccupancyRate.Location = new System.Drawing.Point(600, 35);
            this.lblOccupancyRate.Name = "lblOccupancyRate";
            this.lblOccupancyRate.Size = new System.Drawing.Size(43, 21);
            this.lblOccupancyRate.Text = "0.0%";
            // 
            // dgvReport
            // 
            this.dgvReport.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvReport.Location = new System.Drawing.Point(12, 155);
            this.dgvReport.Name = "dgvReport";
            this.dgvReport.Size = new System.Drawing.Size(860, 335);
            this.dgvReport.TabIndex = 2;
            // 
            // FrmReport
            // 
            this.ClientSize = new System.Drawing.Size(884, 502);
            this.Controls.Add(this.dgvReport);
            this.Controls.Add(this.pnlSummary);
            this.Controls.Add(this.grpFilter);
            this.Name = "FrmReport";
            this.Text = "Báo cáo Doanh thu & Công suất phòng";
            this.Load += new System.EventHandler(this.FrmReport_Load);
            this.grpFilter.ResumeLayout(false);
            this.grpFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudYear)).EndInit();
            this.pnlSummary.ResumeLayout(false);
            this.pnlSummary.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReport)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.GroupBox grpFilter;
        private System.Windows.Forms.Label lblMonth;
        private System.Windows.Forms.ComboBox cboMonth;
        private System.Windows.Forms.Label lblYear;
        private System.Windows.Forms.NumericUpDown nudYear;
        private System.Windows.Forms.Button btnViewReport;
        private System.Windows.Forms.Panel pnlSummary;
        private System.Windows.Forms.Label lblTotalRevenueTitle;
        private System.Windows.Forms.Label lblTotalRevenue;
        private System.Windows.Forms.Label lblTotalBookingsTitle;
        private System.Windows.Forms.Label lblTotalBookings;
        private System.Windows.Forms.Label lblOccupancyTitle;
        private System.Windows.Forms.Label lblOccupancyRate;
        private System.Windows.Forms.DataGridView dgvReport;
    }
}