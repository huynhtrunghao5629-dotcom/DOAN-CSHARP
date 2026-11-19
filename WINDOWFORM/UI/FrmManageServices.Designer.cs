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
            grpInput = new GroupBox();
            btnRefresh = new Button();
            btnDeleteService = new Button();
            btnAddService = new Button();
            txtPrice = new TextBox();
            lblPrice = new Label();
            nudQuantity = new NumericUpDown();
            lblQuantity = new Label();
            cboServices = new ComboBox();
            lblService = new Label();
            cboOccupiedRooms = new ComboBox();
            lblRoom = new Label();
            dgvRoomServices = new DataGridView();
            lblTotalText = new Label();
            lblTotalAmount = new Label();
            grpInput.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudQuantity).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvRoomServices).BeginInit();
            SuspendLayout();
            // 
            // grpInput
            // 
            grpInput.Controls.Add(btnRefresh);
            grpInput.Controls.Add(btnDeleteService);
            grpInput.Controls.Add(btnAddService);
            grpInput.Controls.Add(txtPrice);
            grpInput.Controls.Add(lblPrice);
            grpInput.Controls.Add(nudQuantity);
            grpInput.Controls.Add(lblQuantity);
            grpInput.Controls.Add(cboServices);
            grpInput.Controls.Add(lblService);
            grpInput.Controls.Add(cboOccupiedRooms);
            grpInput.Controls.Add(lblRoom);
            grpInput.Location = new Point(12, 12);
            grpInput.Name = "grpInput";
            grpInput.Size = new Size(860, 188);
            grpInput.TabIndex = 0;
            grpInput.TabStop = false;
            grpInput.Text = "Thêm dịch vụ cho phòng";
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new Point(620, 24);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(220, 28);
            btnRefresh.TabIndex = 7;
            btnRefresh.Text = "Làm mới dữ liệu";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // btnDeleteService
            // 
            btnDeleteService.Location = new Point(730, 68);
            btnDeleteService.Name = "btnDeleteService";
            btnDeleteService.Size = new Size(110, 30);
            btnDeleteService.TabIndex = 6;
            btnDeleteService.Text = "Xóa dịch vụ chọn";
            btnDeleteService.UseVisualStyleBackColor = true;
            btnDeleteService.Click += btnDeleteService_Click;
            // 
            // btnAddService
            // 
            btnAddService.BackColor = Color.FromArgb(40, 167, 69);
            btnAddService.ForeColor = Color.White;
            btnAddService.Location = new Point(620, 68);
            btnAddService.Name = "btnAddService";
            btnAddService.Size = new Size(100, 30);
            btnAddService.TabIndex = 5;
            btnAddService.Text = "+ Thêm dịch vụ";
            btnAddService.UseVisualStyleBackColor = false;
            btnAddService.Click += btnAddService_Click;
            // 
            // txtPrice
            // 
            txtPrice.Location = new Point(470, 72);
            txtPrice.Name = "txtPrice";
            txtPrice.ReadOnly = true;
            txtPrice.Size = new Size(120, 27);
            txtPrice.TabIndex = 4;
            // 
            // lblPrice
            // 
            lblPrice.AutoSize = true;
            lblPrice.Location = new Point(400, 75);
            lblPrice.Name = "lblPrice";
            lblPrice.Size = new Size(65, 20);
            lblPrice.TabIndex = 8;
            lblPrice.Text = "Đơn giá:";
            // 
            // nudQuantity
            // 
            nudQuantity.Location = new Point(470, 27);
            nudQuantity.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudQuantity.Name = "nudQuantity";
            nudQuantity.Size = new Size(120, 27);
            nudQuantity.TabIndex = 3;
            nudQuantity.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // lblQuantity
            // 
            lblQuantity.AutoSize = true;
            lblQuantity.Location = new Point(400, 30);
            lblQuantity.Name = "lblQuantity";
            lblQuantity.Size = new Size(72, 20);
            lblQuantity.TabIndex = 9;
            lblQuantity.Text = "Số lượng:";
            // 
            // cboServices
            // 
            cboServices.DropDownStyle = ComboBoxStyle.DropDownList;
            cboServices.FormattingEnabled = true;
            cboServices.Location = new Point(115, 72);
            cboServices.Name = "cboServices";
            cboServices.Size = new Size(250, 28);
            cboServices.TabIndex = 2;
            cboServices.SelectedIndexChanged += cboServices_SelectedIndexChanged;
            // 
            // lblService
            // 
            lblService.AutoSize = true;
            lblService.Location = new Point(20, 75);
            lblService.Name = "lblService";
            lblService.Size = new Size(97, 20);
            lblService.TabIndex = 10;
            lblService.Text = "Chọn dịch vụ:";
            // 
            // cboOccupiedRooms
            // 
            cboOccupiedRooms.DropDownStyle = ComboBoxStyle.DropDownList;
            cboOccupiedRooms.FormattingEnabled = true;
            cboOccupiedRooms.Location = new Point(115, 27);
            cboOccupiedRooms.Name = "cboOccupiedRooms";
            cboOccupiedRooms.Size = new Size(250, 28);
            cboOccupiedRooms.TabIndex = 1;
            cboOccupiedRooms.SelectedIndexChanged += cboOccupiedRooms_SelectedIndexChanged;
            // 
            // lblRoom
            // 
            lblRoom.AutoSize = true;
            lblRoom.Location = new Point(20, 30);
            lblRoom.Name = "lblRoom";
            lblRoom.Size = new Size(105, 20);
            lblRoom.TabIndex = 11;
            lblRoom.Text = "Phòng đang ở:";
            // 
            // dgvRoomServices
            // 
            dgvRoomServices.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvRoomServices.Location = new Point(12, 206);
            dgvRoomServices.Name = "dgvRoomServices";
            dgvRoomServices.RowHeadersWidth = 51;
            dgvRoomServices.Size = new Size(860, 249);
            dgvRoomServices.TabIndex = 1;
            // 
            // lblTotalText
            // 
            lblTotalText.AutoSize = true;
            lblTotalText.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblTotalText.Location = new Point(550, 468);
            lblTotalText.Name = "lblTotalText";
            lblTotalText.Size = new Size(155, 23);
            lblTotalText.TabIndex = 1;
            lblTotalText.Text = "Tổng tiền dịch vụ:";
            // 
            // lblTotalAmount
            // 
            lblTotalAmount.AutoSize = true;
            lblTotalAmount.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblTotalAmount.ForeColor = Color.Red;
            lblTotalAmount.Location = new Point(695, 467);
            lblTotalAmount.Name = "lblTotalAmount";
            lblTotalAmount.Size = new Size(70, 25);
            lblTotalAmount.TabIndex = 0;
            lblTotalAmount.Text = "0 VNĐ";
            // 
            // FrmManageServices
            // 
            ClientSize = new Size(884, 502);
            Controls.Add(lblTotalAmount);
            Controls.Add(lblTotalText);
            Controls.Add(dgvRoomServices);
            Controls.Add(grpInput);
            Name = "FrmManageServices";
            Text = "Ghi nhận Dịch vụ phát sinh";
            Load += FrmManageServices_Load;
            grpInput.ResumeLayout(false);
            grpInput.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudQuantity).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvRoomServices).EndInit();
            ResumeLayout(false);
            PerformLayout();
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