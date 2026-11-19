namespace HotelManager.UI
{
    partial class FrmCreateBooking
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
            this.lblCustomerName = new System.Windows.Forms.Label();
            this.txtCustomerName = new System.Windows.Forms.TextBox();
            this.lblPhone = new System.Windows.Forms.Label();
            this.txtPhone = new System.Windows.Forms.TextBox();
            this.lblRoom = new System.Windows.Forms.Label();
            this.cboRooms = new System.Windows.Forms.ComboBox();
            this.lblCheckIn = new System.Windows.Forms.Label();
            this.dtpCheckIn = new System.Windows.Forms.DateTimePicker();
            this.lblCheckOut = new System.Windows.Forms.Label();
            this.dtpCheckOut = new System.Windows.Forms.DateTimePicker();
            this.lblNote = new System.Windows.Forms.Label();
            this.txtNote = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblCustomerName
            // 
            this.lblCustomerName.AutoSize = true;
            this.lblCustomerName.Location = new System.Drawing.Point(20, 25);
            this.lblCustomerName.Name = "lblCustomerName";
            this.lblCustomerName.Size = new System.Drawing.Size(93, 15);
            this.lblCustomerName.Text = "Tên khách hàng:";
            // 
            // txtCustomerName
            // 
            this.txtCustomerName.Location = new System.Drawing.Point(125, 22);
            this.txtCustomerName.Name = "txtCustomerName";
            this.txtCustomerName.Size = new System.Drawing.Size(230, 23);
            // 
            // lblPhone
            // 
            this.lblPhone.AutoSize = true;
            this.lblPhone.Location = new System.Drawing.Point(20, 65);
            this.lblPhone.Name = "lblPhone";
            this.lblPhone.Size = new System.Drawing.Size(79, 15);
            this.lblPhone.Text = "Số điện thoại:";
            // 
            // txtPhone
            // 
            this.txtPhone.Location = new System.Drawing.Point(125, 62);
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.Size = new System.Drawing.Size(230, 23);
            // 
            // lblRoom
            // 
            this.lblRoom.AutoSize = true;
            this.lblRoom.Location = new System.Drawing.Point(20, 105);
            this.lblRoom.Name = "lblRoom";
            this.lblRoom.Size = new System.Drawing.Size(75, 15);
            this.lblRoom.Text = "Chọn phòng:";
            // 
            // cboRooms
            // 
            this.cboRooms.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRooms.FormattingEnabled = true;
            this.cboRooms.Location = new System.Drawing.Point(125, 102);
            this.cboRooms.Name = "cboRooms";
            this.cboRooms.Size = new System.Drawing.Size(230, 23);
            // 
            // lblCheckIn
            // 
            this.lblCheckIn.AutoSize = true;
            this.lblCheckIn.Location = new System.Drawing.Point(20, 145);
            this.lblCheckIn.Name = "lblCheckIn";
            this.lblCheckIn.Size = new System.Drawing.Size(102, 15);
            this.lblCheckIn.Text = "Ngày nhận phòng:";
            // 
            // dtpCheckIn
            // 
            this.dtpCheckIn.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpCheckIn.Location = new System.Drawing.Point(125, 142);
            this.dtpCheckIn.Name = "dtpCheckIn";
            this.dtpCheckIn.Size = new System.Drawing.Size(230, 23);
            // 
            // lblCheckOut
            // 
            this.lblCheckOut.AutoSize = true;
            this.lblCheckOut.Location = new System.Drawing.Point(20, 185);
            this.lblCheckOut.Name = "lblCheckOut";
            this.lblCheckOut.Size = new System.Drawing.Size(92, 15);
            this.lblCheckOut.Text = "Ngày trả phòng:";
            // 
            // dtpCheckOut
            // 
            this.dtpCheckOut.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpCheckOut.Location = new System.Drawing.Point(125, 182);
            this.dtpCheckOut.Name = "dtpCheckOut";
            this.dtpCheckOut.Size = new System.Drawing.Size(230, 23);
            // 
            // lblNote
            // 
            this.lblNote.AutoSize = true;
            this.lblNote.Location = new System.Drawing.Point(20, 225);
            this.lblNote.Name = "lblNote";
            this.lblNote.Size = new System.Drawing.Size(51, 15);
            this.lblNote.Text = "Ghi chú:";
            // 
            // txtNote
            // 
            this.txtNote.Location = new System.Drawing.Point(125, 222);
            this.txtNote.Multiline = true;
            this.txtNote.Name = "txtNote";
            this.txtNote.Size = new System.Drawing.Size(230, 60);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(125, 300);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(110, 30);
            this.btnSave.Text = "Tạo đơn";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(245, 300);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(110, 30);
            this.btnCancel.Text = "Hủy bỏ";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // FrmCreateBooking
            // 
            this.ClientSize = new System.Drawing.Size(384, 351);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtNote);
            this.Controls.Add(this.lblNote);
            this.Controls.Add(this.dtpCheckOut);
            this.Controls.Add(this.lblCheckOut);
            this.Controls.Add(this.dtpCheckIn);
            this.Controls.Add(this.lblCheckIn);
            this.Controls.Add(this.cboRooms);
            this.Controls.Add(this.lblRoom);
            this.Controls.Add(this.txtPhone);
            this.Controls.Add(this.lblPhone);
            this.Controls.Add(this.txtCustomerName);
            this.Controls.Add(this.lblCustomerName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmCreateBooking";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Tạo đơn đặt phòng tại quầy";
            this.Load += new System.EventHandler(this.FrmCreateBooking_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblCustomerName;
        private System.Windows.Forms.TextBox txtCustomerName;
        private System.Windows.Forms.Label lblPhone;
        private System.Windows.Forms.TextBox txtPhone;
        private System.Windows.Forms.Label lblRoom;
        private System.Windows.Forms.ComboBox cboRooms;
        private System.Windows.Forms.Label lblCheckIn;
        private System.Windows.Forms.DateTimePicker dtpCheckIn;
        private System.Windows.Forms.Label lblCheckOut;
        private System.Windows.Forms.DateTimePicker dtpCheckOut;
        private System.Windows.Forms.Label lblNote;
        private System.Windows.Forms.TextBox txtNote;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
    }
}