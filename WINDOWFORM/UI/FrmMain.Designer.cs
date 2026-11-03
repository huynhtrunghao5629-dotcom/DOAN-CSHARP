namespace HotelManager.UI
{
    partial class FrmMain
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
            menuStrip1 = new MenuStrip();
            menuStrip2 = new MenuStrip();
            mnuManageRooms = new ToolStripMenuItem();
            mnuManageCustomers = new ToolStripMenuItem();
            mnuReports = new ToolStripMenuItem();
            hệThốngToolStripMenuItem = new ToolStripMenuItem();
            mnuLogout = new ToolStripMenuItem();
            mnuExit = new ToolStripMenuItem();
            menuStrip2.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Location = new Point(0, 28);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // menuStrip2
            // 
            menuStrip2.ImageScalingSize = new Size(20, 20);
            menuStrip2.Items.AddRange(new ToolStripItem[] { mnuManageRooms, mnuManageCustomers, mnuReports, hệThốngToolStripMenuItem });
            menuStrip2.Location = new Point(0, 0);
            menuStrip2.Name = "menuStrip2";
            menuStrip2.Size = new Size(800, 28);
            menuStrip2.TabIndex = 1;
            menuStrip2.Text = "menuStrip2";
            // 
            // mnuManageRooms
            // 
            mnuManageRooms.Name = "mnuManageRooms";
            mnuManageRooms.Size = new Size(117, 24);
            mnuManageRooms.Text = "Quản lí phòng";
            mnuManageRooms.Click += mnuManageRooms_Click;
            // 
            // mnuManageCustomers
            // 
            mnuManageCustomers.Name = "mnuManageCustomers";
            mnuManageCustomers.Size = new Size(152, 24);
            mnuManageCustomers.Text = "Quản lý khách hàng";
            // 
            // mnuReports
            // 
            mnuReports.Name = "mnuReports";
            mnuReports.Size = new Size(152, 24);
            mnuReports.Text = "Báo cáo - Thống kê";
            // 
            // hệThốngToolStripMenuItem
            // 
            hệThốngToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { mnuLogout, mnuExit });
            hệThốngToolStripMenuItem.Name = "hệThốngToolStripMenuItem";
            hệThốngToolStripMenuItem.Size = new Size(85, 24);
            hệThốngToolStripMenuItem.Text = "Hệ thống";
            // 
            // mnuLogout
            // 
            mnuLogout.Name = "mnuLogout";
            mnuLogout.Size = new Size(160, 26);
            mnuLogout.Text = "Đăng xuất";
            // 
            // mnuExit
            // 
            mnuExit.Name = "mnuExit";
            mnuExit.Size = new Size(160, 26);
            mnuExit.Text = "Thoát";
            // 
            // FrmMain
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(menuStrip1);
            Controls.Add(menuStrip2);
            IsMdiContainer = true;
            MainMenuStrip = menuStrip1;
            Name = "FrmMain";
            Text = "FrmMain";
            Load += FrmMain_Load;
            menuStrip2.ResumeLayout(false);
            menuStrip2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private MenuStrip menuStrip2;
        private ToolStripMenuItem mnuManageRooms;
        private ToolStripMenuItem mnuManageCustomers;
        private ToolStripMenuItem mnuReports;
        private ToolStripMenuItem hệThốngToolStripMenuItem;
        private ToolStripMenuItem đăngXuấtToolStripMenuItem;
        private ToolStripMenuItem thoátToolStripMenuItem;
        private ToolStripMenuItem mnuLogout;
        private ToolStripMenuItem mnuExit;
    }
}