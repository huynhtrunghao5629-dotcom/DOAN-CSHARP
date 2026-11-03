namespace HotelManager.UI
{
    partial class FrmManageServices
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
            this.grpInput = new System.Windows.Forms.GroupBox();
            this.lblRoom = new System.Windows.Forms.Label();
            this.cboOccupiedRooms = new System.Windows.Forms.ComboBox();
            this.lblService = new System.Windows.Forms.Label();
            this.cboServices = new System.Windows.Forms.ComboBox();
            this.lblQuantity = new System.Windows.Forms.Label();
            this.nudQuantity = new System.Windows.Forms.NumericUpDown();
            this.lblPrice = new System.Windows.Forms.Label();
            this.txtPrice = new System.Windows.Forms.TextBox();
            this.btnAddService = new System.Windows.Forms.Button();
            this.btnDeleteService = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.dgvRoomServices = new System.Windows.Forms.DataGridView();
            this.lblTotalText = new System.Windows.Forms.Label();
            this.lblTotalAmount = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudQuantity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRoomServices)).BeginInit();
            this.grpInput.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpInput
            // 
            this.grpInput.Controls.Add(this.btnRefresh);
            this.grpInput.Controls.Add(this.btnDeleteService);
            this.grpInput.Controls.Add(this.btnAddService);
            this.grpInput.Controls.Add(this.txtPrice);
            this.grpInput.Controls.Add(this.lblPrice);
            this.grpInput.Controls.Add(this.nudQuantity);
            this.grpInput.Controls.Add(this.lblQuantity);
            this.grpInput.Controls.Add(this.cboServices);
            this.grpInput.Controls.Add(this.lblService);
            this.grpInput.Controls.Add(this.cboOccupiedRooms);
            this.grpInput.Controls.Add(this.lblRoom);
            this.grpInput.Location = new System.Drawing.Point(12, 12);
            this.grpInput.Name = "grpInput";
            this.grpInput.Size = new System.Drawing.Size(860, 130);
            this.grpInput.TabIndex = 0;
            this.grpInput.TabStop = false;
            this.grpInput.Text = "Thêm dịch vụ cho phòng";
            // 
            // lblRoom
            // 
            this.lblRoom.AutoSize = true;
            this.lblRoom.Location = new System.Drawing.Point(20, 30);
            this.lblRoom.Name = "lblRoom";
            this.lblRoom.Size = new System.Drawing.Size(76, 15);
            this.lblRoom.Text = "Phòng đang ở:";
            // 
            // cboOccupiedRooms
            // 
            this.cboOccupiedRooms.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboOccupiedRooms.FormattingEnabled = true;
            this.cboOccupiedRooms.Location = new System.Drawing.Point(115, 27);
            this.cboOccupiedRooms.Name = "cboOccupiedRooms";
            this.cboOccupiedRooms.Size = new System.Drawing.Size(250, 23);
            this.cboOccupiedRooms.TabIndex = 1;
            this.cboOccupiedRooms.SelectedIndexChanged += new System.EventHandler(this.cboOccupiedRooms_SelectedIndexChanged);
            // 
            // lblService
            // 
            this.lblService.AutoSize = true;
            this.lblService.Location = new System.Drawing.Point(20, 75);
            this.lblService.Name = "lblService";
            this.lblService.Size = new System.Drawing.Size(77, 15);
            this.lblService.Text = "Chọn dịch vụ:";
            // 
            // cboServices
            // 
            this.cboServices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboServices.FormattingEnabled = true;
            this.cboServices.Location = new System.Drawing.Point(115, 72);
            this.cboServices.Name = "cboServices";
            this.cboServices.Size = new System.Drawing.Size(250, 23);
            this.cboServices.TabIndex = 2;
            this.cboServices.SelectedIndexChanged += new System.EventHandler(this.cboServices_SelectedIndexChanged);
            // 
            // lblQuantity
            // 
            this.lblQuantity.AutoSize = true;
            this.lblQuantity.Location = new System.Drawing.Point(400, 30);
            this.lblQuantity.Name = "lblQuantity";
            this.lblQuantity.Size = new System.Drawing.Size(57, 15);
            this.lblQuantity.Text = "Số lượng:";
            // 
            // nudQuantity
            // 
            this.nudQuantity.Location = new System.Drawing.Point(470, 27);
            this.nudQuantity.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudQuantity.Name = "nudQuantity";
            this.nudQuantity.Size = new System.Drawing.Size(120, 23);
            this.nudQuantity.TabIndex = 3;
            this.nudQuantity.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // lblPrice
            // 
            this.lblPrice.AutoSize = true;
            this.lblPrice.Location = new System.Drawing.Point(400, 75);
            this.lblPrice.Name = "lblPrice";
            this.lblPrice.Size = new System.Drawing.Size(51, 15);
            this.lblPrice.Text = "Đơn giá:";
            // 
            // txtPrice
            // 
            this.txtPrice.Location = new System.Drawing.Point(470, 72);
            this.txtPrice.Name = "txtPrice";
            this.txtPrice.ReadOnly = true;
            this.txtPrice.Size = new System.Drawing.Size(120, 23);
            this.txtPrice.TabIndex = 4;
            // 
            // btnAddService
            // 
            this.btnAddService.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.btnAddService.ForeColor = System.Drawing.Color.White;
            this.btnAddService.Location = new System.Drawing.Point(620, 68);
            this.btnAddService.Name = "btnAddService";
            this.btnAddService.Size = new System.Drawing.Size(100, 30);
            this.btnAddService.TabIndex = 5;
            this.btnAddService.Text = "+ Thêm dịch vụ";
            this.btnAddService.UseVisualStyleBackColor = false;
            this.btnAddService.Click += new System.EventHandler(this.btnAddService_Click);
            // 
            // btnDeleteService
            // 
            this.btnDeleteService.Location = new System.Drawing.Point(730, 68);
            this.btnDeleteService.Name = "btnDeleteService";
            this.btnDeleteService.Size = new System.Drawing.Size(110, 30);
            this.btnDeleteService.TabIndex = 6;
            this.btnDeleteService.Text = "Xóa dịch vụ chọn";
            this.btnDeleteService.UseVisualStyleBackColor = true;
            this.btnDeleteService.Click += new System.EventHandler(this.btnDeleteService_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(620, 24);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(220, 28);
            this.btnRefresh.TabIndex = 7;
            this.btnRefresh.Text = "Làm mới dữ liệu";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // dgvRoomServices
            // 
            this.dgvRoomServices.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRoomServices.Location = new System.Drawing.Point(12, 155);
            this.dgvRoomServices.Name = "dgvRoomServices";
            this.dgvRoomServices.Size = new System.Drawing.Size(860, 300);
            this.dgvRoomServices.TabIndex = 1;
            // 
            // lblTotalText
            // 
            this.lblTotalText.AutoSize = true;
            this.lblTotalText.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblTotalText.Location = new System.Drawing.Point(550, 468);
            this.lblTotalText.Name = "lblTotalText";
            this.lblTotalText.Size = new System.Drawing.Size(135, 19);
            this.lblTotalText.Text = "Tổng tiền dịch vụ:";
            // 
            // lblTotalAmount
            // 
            this.lblTotalAmount.AutoSize = true;
            this.lblTotalAmount.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblTotalAmount.ForeColor = System.Drawing.Color.Red;
            this.lblTotalAmount.Location = new System.Drawing.Point(695, 467);
            this.lblTotalAmount.Name = "lblTotalAmount";
            this.lblTotalAmount.Size = new System.Drawing.Size(55, 20);
            this.lblTotalAmount.Text = "0 VNĐ";
            // 
            // FrmManageServices
            // 
            this.ClientSize = new System.Drawing.Size(884, 502);
            this.Controls.Add(this.lblTotalAmount);
            this.Controls.Add(this.lblTotalText);
            this.Controls.Add(this.dgvRoomServices);
            this.Controls.Add(this.grpInput);
            this.Name = "FrmManageServices";
            this.Text = "Ghi nhận Dịch vụ phát sinh";
            this.Load += new System.EventHandler(this.FrmManageServices_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nudQuantity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRoomServices)).EndInit();
            this.grpInput.ResumeLayout(false);
            this.grpInput.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.GroupBox grpInput;
        private System.Windows.Forms.Label lblRoom;
        private System.Windows.Forms.ComboBox cboOccupiedRooms;
        private System.Windows.Forms.Label lblService;
        private System.Windows.Forms.ComboBox cboServices;
        private System.Windows.Forms.Label lblQuantity;
        private System.Windows.Forms.NumericUpDown nudQuantity;
        private System.Windows.Forms.Label lblPrice;
        private System.Windows.Forms.TextBox txtPrice;
        private System.Windows.Forms.Button btnAddService;
        private System.Windows.Forms.Button btnDeleteService;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.DataGridView dgvRoomServices;
        private System.Windows.Forms.Label lblTotalText;
        private System.Windows.Forms.Label lblTotalAmount;
    }
}