namespace HotelManager.UI
{
    partial class FrmManageRooms
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
            this.dgvRooms = new System.Windows.Forms.DataGridView();
            this.lblSoPhong = new System.Windows.Forms.Label();
            this.txtSoPhong = new System.Windows.Forms.TextBox();
            this.lblLoaiPhong = new System.Windows.Forms.Label();
            this.txtTenLoaiPhong = new System.Windows.Forms.TextBox();
            this.lblGiaPhong = new System.Windows.Forms.Label();
            this.txtGiaPhong = new System.Windows.Forms.TextBox();
            this.lblTrangThai = new System.Windows.Forms.Label();
            this.cboTrangThai = new System.Windows.Forms.ComboBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.grpInput = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRooms)).BeginInit();
            this.grpInput.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvRooms
            // 
            this.dgvRooms.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRooms.Location = new System.Drawing.Point(12, 170);
            this.dgvRooms.Name = "dgvRooms";
            this.dgvRooms.Size = new System.Drawing.Size(860, 320);
            this.dgvRooms.TabIndex = 0;
            this.dgvRooms.SelectionChanged += new System.EventHandler(this.dgvRooms_SelectionChanged);
            // 
            // grpInput
            // 
            this.grpInput.Controls.Add(this.btnClear);
            this.grpInput.Controls.Add(this.btnRefresh);
            this.grpInput.Controls.Add(this.btnDelete);
            this.grpInput.Controls.Add(this.btnEdit);
            this.grpInput.Controls.Add(this.btnAdd);
            this.grpInput.Controls.Add(this.cboTrangThai);
            this.grpInput.Controls.Add(this.lblTrangThai);
            this.grpInput.Controls.Add(this.txtGiaPhong);
            this.grpInput.Controls.Add(this.lblGiaPhong);
            this.grpInput.Controls.Add(this.txtTenLoaiPhong);
            this.grpInput.Controls.Add(this.lblLoaiPhong);
            this.grpInput.Controls.Add(this.txtSoPhong);
            this.grpInput.Controls.Add(this.lblSoPhong);
            this.grpInput.Location = new System.Drawing.Point(12, 12);
            this.grpInput.Name = "grpInput";
            this.grpInput.Size = new System.Drawing.Size(860, 140);
            this.grpInput.TabIndex = 1;
            this.grpInput.TabStop = false;
            this.grpInput.Text = "Thông tin phòng";
            // 
            // lblSoPhong
            // 
            this.lblSoPhong.AutoSize = true;
            this.lblSoPhong.Location = new System.Drawing.Point(20, 30);
            this.lblSoPhong.Name = "lblSoPhong";
            this.lblSoPhong.Size = new System.Drawing.Size(61, 15);
            this.lblSoPhong.Text = "Số phòng:";
            // 
            // txtSoPhong
            // 
            this.txtSoPhong.Location = new System.Drawing.Point(100, 27);
            this.txtSoPhong.Name = "txtSoPhong";
            this.txtSoPhong.Size = new System.Drawing.Size(260, 23);
            this.txtSoPhong.TabIndex = 1;
            // 
            // lblLoaiPhong
            // 
            this.lblLoaiPhong.AutoSize = true;
            this.lblLoaiPhong.Location = new System.Drawing.Point(20, 70);
            this.lblLoaiPhong.Name = "lblLoaiPhong";
            this.lblLoaiPhong.Size = new System.Drawing.Size(70, 15);
            this.lblLoaiPhong.Text = "Loại phòng:";
            // 
            // txtTenLoaiPhong
            // 
            this.txtTenLoaiPhong.Location = new System.Drawing.Point(100, 67);
            this.txtTenLoaiPhong.Name = "txtTenLoaiPhong";
            this.txtTenLoaiPhong.Size = new System.Drawing.Size(260, 23);
            this.txtTenLoaiPhong.TabIndex = 2;
            // 
            // lblGiaPhong
            // 
            this.lblGiaPhong.AutoSize = true;
            this.lblGiaPhong.Location = new System.Drawing.Point(400, 30);
            this.lblGiaPhong.Name = "lblGiaPhong";
            this.lblGiaPhong.Size = new System.Drawing.Size(65, 15);
            this.lblGiaPhong.Text = "Giá phòng:";
            // 
            // txtGiaPhong
            // 
            this.txtGiaPhong.Location = new System.Drawing.Point(480, 27);
            this.txtGiaPhong.Name = "txtGiaPhong";
            this.txtGiaPhong.Size = new System.Drawing.Size(260, 23);
            this.txtGiaPhong.TabIndex = 3;
            // 
            // lblTrangThai
            // 
            this.lblTrangThai.AutoSize = true;
            this.lblTrangThai.Location = new System.Drawing.Point(400, 70);
            this.lblTrangThai.Name = "lblTrangThai";
            this.lblTrangThai.Size = new System.Drawing.Size(62, 15);
            this.lblTrangThai.Text = "Trạng thái:";
            // 
            // cboTrangThai
            // 
            this.cboTrangThai.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTrangThai.FormattingEnabled = true;
            this.cboTrangThai.Location = new System.Drawing.Point(480, 67);
            this.cboTrangThai.Name = "cboTrangThai";
            this.cboTrangThai.Size = new System.Drawing.Size(260, 23);
            this.cboTrangThai.TabIndex = 4;
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(100, 103);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(90, 28);
            this.btnAdd.TabIndex = 5;
            this.btnAdd.Text = "Thêm mới";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(200, 103);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(90, 28);
            this.btnEdit.TabIndex = 6;
            this.btnEdit.Text = "Cập nhật";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(300, 103);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(90, 28);
            this.btnDelete.TabIndex = 7;
            this.btnDelete.Text = "Xóa";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(400, 103);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(90, 28);
            this.btnClear.TabIndex = 8;
            this.btnClear.Text = "Làm trống";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(500, 103);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(90, 28);
            this.btnRefresh.TabIndex = 9;
            this.btnRefresh.Text = "Tải lại";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // FrmManageRooms
            // 
            this.ClientSize = new System.Drawing.Size(884, 502);
            this.Controls.Add(this.grpInput);
            this.Controls.Add(this.dgvRooms);
            this.Name = "FrmManageRooms";
            this.Text = "Quản lý Phòng";
            this.Load += new System.EventHandler(this.FrmManageRooms_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRooms)).EndInit();
            this.grpInput.ResumeLayout(false);
            this.grpInput.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.DataGridView dgvRooms;
        private System.Windows.Forms.GroupBox grpInput;
        private System.Windows.Forms.Label lblSoPhong;
        private System.Windows.Forms.TextBox txtSoPhong;
        private System.Windows.Forms.Label lblLoaiPhong;
        private System.Windows.Forms.TextBox txtTenLoaiPhong;
        private System.Windows.Forms.Label lblGiaPhong;
        private System.Windows.Forms.TextBox txtGiaPhong;
        private System.Windows.Forms.Label lblTrangThai;
        private System.Windows.Forms.ComboBox cboTrangThai;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnRefresh;
    }
}