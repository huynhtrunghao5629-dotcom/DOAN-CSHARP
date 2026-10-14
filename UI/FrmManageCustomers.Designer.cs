namespace HotelManagement.Desktop.UI
{
    partial class FrmManageCustomers
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
            dgvCustomers = new System.Windows.Forms.DataGridView();
            txtFullName = new System.Windows.Forms.TextBox();
            txtPhone = new System.Windows.Forms.TextBox();
            txtEmail = new System.Windows.Forms.TextBox();
            txtIdentityCard = new System.Windows.Forms.TextBox();
            txtSearch = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            btnAdd = new System.Windows.Forms.Button();
            btnSave = new System.Windows.Forms.Button();
            btnDelete = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(dgvCustomers)).BeginInit();
            SuspendLayout();
            // 
            // dgvCustomers
            // 
            dgvCustomers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvCustomers.Dock = System.Windows.Forms.DockStyle.Bottom;
            dgvCustomers.Location = new System.Drawing.Point(0, 262);
            dgvCustomers.Name = "dgvCustomers";
            dgvCustomers.RowHeadersWidth = 51;
            dgvCustomers.Size = new System.Drawing.Size(800, 188);
            dgvCustomers.TabIndex = 0;
            dgvCustomers.CellClick += dgvCustomers_CellClick;
            dgvCustomers.DataError += dgvCustomers_DataError;
            // 
            // txtFullName
            // 
            txtFullName.Location = new System.Drawing.Point(276, 12);
            txtFullName.Name = "txtFullName";
            txtFullName.Size = new System.Drawing.Size(125, 27);
            txtFullName.TabIndex = 1;
            // 
            // txtPhone
            // 
            txtPhone.Location = new System.Drawing.Point(276, 70);
            txtPhone.Name = "txtPhone";
            txtPhone.Size = new System.Drawing.Size(125, 27);
            txtPhone.TabIndex = 2;
            // 
            // txtEmail
            // 
            txtEmail.Location = new System.Drawing.Point(276, 118);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new System.Drawing.Size(125, 27);
            txtEmail.TabIndex = 3;
            // 
            // txtIdentityCard
            // 
            txtIdentityCard.Location = new System.Drawing.Point(276, 174);
            txtIdentityCard.Name = "txtIdentityCard";
            txtIdentityCard.Size = new System.Drawing.Size(125, 27);
            txtIdentityCard.TabIndex = 4;
            // 
            // txtSearch
            // 
            txtSearch.Location = new System.Drawing.Point(276, 226);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new System.Drawing.Size(125, 27);
            txtSearch.TabIndex = 5;
            txtSearch.TextChanged += txtSearch_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(163, 19);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(57, 20);
            label1.TabIndex = 6;
            label1.Text = "Họ tên:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(163, 73);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(100, 20);
            label2.TabIndex = 7;
            label2.Text = "Số điện thoại:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(163, 125);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(49, 20);
            label3.TabIndex = 8;
            label3.Text = "Email:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(163, 181);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(100, 20);
            label4.TabIndex = 9;
            label4.Text = "CCCD/CMND:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(163, 229);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(73, 20);
            label5.TabIndex = 10;
            label5.Text = "Tìm kiếm:";
            // 
            // btnAdd
            // 
            btnAdd.Location = new System.Drawing.Point(527, 19);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new System.Drawing.Size(124, 52);
            btnAdd.TabIndex = 11;
            btnAdd.Text = "Reset";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += btnAdd_Click;
            // 
            // btnSave
            // 
            btnSave.Location = new System.Drawing.Point(527, 95);
            btnSave.Name = "btnSave";
            btnSave.Size = new System.Drawing.Size(124, 50);
            btnSave.TabIndex = 12;
            btnSave.Text = "Lưu";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnDelete
            // 
            btnDelete.Location = new System.Drawing.Point(527, 166);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new System.Drawing.Size(124, 50);
            btnDelete.TabIndex = 13;
            btnDelete.Text = "Xóa";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;
            // 
            // FrmManageCustomers
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 450);
            Controls.Add(btnDelete);
            Controls.Add(btnSave);
            Controls.Add(btnAdd);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(txtSearch);
            Controls.Add(txtIdentityCard);
            Controls.Add(txtEmail);
            Controls.Add(txtPhone);
            Controls.Add(txtFullName);
            Controls.Add(dgvCustomers);
            Name = "FrmManageCustomers";
            Text = "FrmManageCustomers";
            Load += FrmManageCustomers_Load;
            ((System.ComponentModel.ISupportInitialize)(dgvCustomers)).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.DataGridView dgvCustomers;
        private System.Windows.Forms.TextBox txtFullName;
        private System.Windows.Forms.TextBox txtPhone;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.TextBox txtIdentityCard;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnDelete;
    }
}