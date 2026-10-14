namespace HotelManager.UI
{
    partial class FrmManageRooms
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtRoomNumber = new TextBox();
            label1 = new Label();
            label2 = new Label();
            txtPrice = new TextBox();
            txtStatus = new ComboBox();
            label3 = new Label();
            btnAdd = new Button();
            btnSave = new Button();
            btnDelete = new Button();
            txtSearch = new TextBox();
            label4 = new Label();
            groupBox1 = new GroupBox();
            dgvRooms = new DataGridView();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvRooms).BeginInit();
            SuspendLayout();
            // 
            // txtRoomNumber
            // 
            txtRoomNumber.Location = new Point(463, 26);
            txtRoomNumber.Name = "txtRoomNumber";
            txtRoomNumber.Size = new Size(290, 27);
            txtRoomNumber.TabIndex = 1;
            txtRoomNumber.TextChanged += txtRoomNumber_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(366, 33);
            label1.Name = "label1";
            label1.Size = new Size(73, 20);
            label1.TabIndex = 2;
            label1.Text = "Số phòng";
            label1.Click += label1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(370, 77);
            label2.Name = "label2";
            label2.Size = new Size(69, 20);
            label2.TabIndex = 3;
            label2.Text = "Giá/Đêm";
            // 
            // txtPrice
            // 
            txtPrice.Location = new Point(463, 70);
            txtPrice.Name = "txtPrice";
            txtPrice.Size = new Size(290, 27);
            txtPrice.TabIndex = 4;

            // 
            // txtStatus
            // 
            txtStatus.FormattingEnabled = true;
            txtStatus.Location = new Point(463, 107);
            txtStatus.Name = "txtStatus";
            txtStatus.Size = new Size(290, 28);
            txtStatus.TabIndex = 5;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(366, 115);
            label3.Name = "label3";
            label3.Size = new Size(75, 20);
            label3.TabIndex = 6;
            label3.Text = "Trạng thái";
            label3.Click += label3_Click;
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(898, 87);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(166, 38);
            btnAdd.TabIndex = 7;
            btnAdd.Text = "Reset";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += btnAdd_Click;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(898, 26);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(166, 41);
            btnSave.TabIndex = 8;
            btnSave.Text = "Lưu";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnDelete
            // 
            btnDelete.Location = new Point(898, 139);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(166, 47);
            btnDelete.TabIndex = 9;
            btnDelete.Text = "Xóa";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;
            // 
            // txtSearch
            // 
            txtSearch.Location = new Point(463, 159);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(290, 27);
            txtSearch.TabIndex = 10;
            txtSearch.TextChanged += txtSearch_TextChanged;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(318, 159);
            label4.Name = "label4";
            label4.Size = new Size(136, 20);
            label4.TabIndex = 11;
            label4.Text = "Tìm kiếm số phòng";
            label4.Click += label4_Click;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(txtSearch);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(txtStatus);
            groupBox1.Controls.Add(btnAdd);
            groupBox1.Controls.Add(txtPrice);
            groupBox1.Controls.Add(btnSave);
            groupBox1.Controls.Add(txtRoomNumber);
            groupBox1.Controls.Add(btnDelete);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label3);
            groupBox1.Location = new Point(0, 0);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(1812, 508);
            groupBox1.TabIndex = 12;
            groupBox1.TabStop = false;
            groupBox1.Text = "Thông tin - Công cụ Phòng";
            groupBox1.Enter += groupBox1_Enter;
            // 
            // dgvRooms
            // 
            dgvRooms.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvRooms.Dock = DockStyle.Fill;
            dgvRooms.Location = new Point(0, 0);
            dgvRooms.Name = "dgvRooms";
            dgvRooms.RowHeadersWidth = 51;
            dgvRooms.Size = new Size(1812, 659);
            dgvRooms.TabIndex = 0;
            dgvRooms.CellClick += dgvRooms_CellClick;
            dgvRooms.CellContentClick += dataGridView1_CellContentClick;
            dgvRooms.DataError += dgvRooms_DataError;
            // 
            // FrmManageRooms
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1812, 659);
            Controls.Add(groupBox1);
            Controls.Add(dgvRooms);
            Name = "FrmManageRooms";
            Text = "FrmManageRooms";
            Load += FrmManageRooms_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvRooms).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private TextBox txtRoomNumber;
        private Label label1;
        private Label label2;
        private TextBox txtPrice;
        private ComboBox txtStatus;
        private Label label3;
        private Button btnAdd;
        private Button btnSave;
        private Button btnDelete;
        private TextBox txtSearch;
        private Label label4;
        private GroupBox groupBox1;
        private DataGridView dgvRooms;
    }
}