namespace HotelManager.UI
{
    partial class FrmRoomGrid
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
            this.flpRooms = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlLegend = new System.Windows.Forms.Panel();
            this.lblLegendMaintenance = new System.Windows.Forms.Label();
            this.pnlMaintenance = new System.Windows.Forms.Panel();
            this.lblLegendOccupied = new System.Windows.Forms.Label();
            this.pnlOccupied = new System.Windows.Forms.Panel();
            this.lblLegendAvailable = new System.Windows.Forms.Label();
            this.pnlAvailable = new System.Windows.Forms.Panel();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.pnlLegend.SuspendLayout();
            this.SuspendLayout();
            // 
            // flpRooms
            // 
            this.flpRooms.AutoScroll = true;
            this.flpRooms.BackColor = System.Drawing.Color.WhiteSmoke;
            this.flpRooms.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flpRooms.Location = new System.Drawing.Point(12, 60);
            this.flpRooms.Name = "flpRooms";
            this.flpRooms.Size = new System.Drawing.Size(860, 430);
            this.flpRooms.TabIndex = 0;
            // 
            // pnlLegend
            // 
            this.pnlLegend.Controls.Add(this.btnRefresh);
            this.pnlLegend.Controls.Add(this.lblLegendMaintenance);
            this.pnlLegend.Controls.Add(this.pnlMaintenance);
            this.pnlLegend.Controls.Add(this.lblLegendOccupied);
            this.pnlLegend.Controls.Add(this.pnlOccupied);
            this.pnlLegend.Controls.Add(this.lblLegendAvailable);
            this.pnlLegend.Controls.Add(this.pnlAvailable);
            this.pnlLegend.Location = new System.Drawing.Point(12, 12);
            this.pnlLegend.Name = "pnlLegend";
            this.pnlLegend.Size = new System.Drawing.Size(860, 40);
            this.pnlLegend.TabIndex = 1;
            // 
            // pnlAvailable
            // 
            this.pnlAvailable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.pnlAvailable.Location = new System.Drawing.Point(15, 10);
            this.pnlAvailable.Name = "pnlAvailable";
            this.pnlAvailable.Size = new System.Drawing.Size(20, 20);
            this.pnlAvailable.TabIndex = 0;
            // 
            // lblLegendAvailable
            // 
            this.lblLegendAvailable.AutoSize = true;
            this.lblLegendAvailable.Location = new System.Drawing.Point(40, 12);
            this.lblLegendAvailable.Name = "lblLegendAvailable";
            this.lblLegendAvailable.Size = new System.Drawing.Size(73, 15);
            this.lblLegendAvailable.Text = "Phòng Trống";
            // 
            // pnlOccupied
            // 
            this.pnlOccupied.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.pnlOccupied.Location = new System.Drawing.Point(140, 10);
            this.pnlOccupied.Name = "pnlOccupied";
            this.pnlOccupied.Size = new System.Drawing.Size(20, 20);
            this.pnlOccupied.TabIndex = 1;
            // 
            // lblLegendOccupied
            // 
            this.lblLegendOccupied.AutoSize = true;
            this.lblLegendOccupied.Location = new System.Drawing.Point(165, 12);
            this.lblLegendOccupied.Name = "lblLegendOccupied";
            this.lblLegendOccupied.Size = new System.Drawing.Size(86, 15);
            this.lblLegendOccupied.Text = "Đang sử dụng";
            // 
            // pnlMaintenance
            // 
            this.pnlMaintenance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(193)))), ((int)(((byte)(7)))));
            this.pnlMaintenance.Location = new System.Drawing.Point(275, 10);
            this.pnlMaintenance.Name = "pnlMaintenance";
            this.pnlMaintenance.Size = new System.Drawing.Size(20, 20);
            this.pnlMaintenance.TabIndex = 2;
            // 
            // lblLegendMaintenance
            // 
            this.lblLegendMaintenance.AutoSize = true;
            this.lblLegendMaintenance.Location = new System.Drawing.Point(300, 12);
            this.lblLegendMaintenance.Name = "lblLegendMaintenance";
            this.lblLegendMaintenance.Size = new System.Drawing.Size(77, 15);
            this.lblLegendMaintenance.Text = "Đang bảo trì";
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(750, 6);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(95, 28);
            this.btnRefresh.TabIndex = 3;
            this.btnRefresh.Text = "Làm mới sơ đồ";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // FrmRoomGrid
            // 
            this.ClientSize = new System.Drawing.Size(884, 502);
            this.Controls.Add(this.pnlLegend);
            this.Controls.Add(this.flpRooms);
            this.Name = "FrmRoomGrid";
            this.Text = "Sơ đồ Phòng (Trực quan)";
            this.Load += new System.EventHandler(this.FrmRoomGrid_Load);
            this.pnlLegend.ResumeLayout(false);
            this.pnlLegend.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flpRooms;
        private System.Windows.Forms.Panel pnlLegend;
        private System.Windows.Forms.Panel pnlAvailable;
        private System.Windows.Forms.Label lblLegendAvailable;
        private System.Windows.Forms.Panel pnlOccupied;
        private System.Windows.Forms.Label lblLegendOccupied;
        private System.Windows.Forms.Panel pnlMaintenance;
        private System.Windows.Forms.Label lblLegendMaintenance;
        private System.Windows.Forms.Button btnRefresh;
    }
}