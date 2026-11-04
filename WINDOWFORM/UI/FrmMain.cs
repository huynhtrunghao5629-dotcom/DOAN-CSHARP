using System;
using System.Windows.Forms;
using HotelManagement.Desktop.Models;
using HotelManager.Services;

namespace HotelManager.UI
{
    public partial class FrmMain : Form
    {
        private readonly UserSession _user;

        public FrmMain(UserSession user)
        {
            InitializeComponent();
            _user = user;
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            this.Text = $"Quản lý khách sạn - Tài khoản: {_user.Username} ({TrangThaiTiengViet.VaiTro(_user.Role)})";

            bool isAdmin = string.Equals(_user.Role, "Admin", StringComparison.OrdinalIgnoreCase);

            // Phân quyền Menu quản lý
            if (Controls.Find("mnuManagerRooms", true).Length > 0)
                Controls.Find("mnuManagerRooms", true)[0].Enabled = isAdmin;

            // Đăng ký sự kiện Đăng xuất & Thoát để giải quyết warning CS0169
            if (đăngXuấtToolStripMenuItem != null)
                đăngXuấtToolStripMenuItem.Click += đăngXuấtToolStripMenuItem_Click;

            if (thoátToolStripMenuItem != null)
                thoátToolStripMenuItem.Click += thoátToolStripMenuItem_Click;
        }

        // Khớp chính xác với tên hàm mnuManagerRooms_Click trong FrmMain.Designer.cs (Dòng 66)
        public void mnuManagerRooms_Click(object sender, EventArgs e)
        {
            FrmManageRooms frm = new FrmManageRooms();
            frm.MdiParent = this;
            frm.Show();
        }

        public void mnuManageRooms_Click(object sender, EventArgs e)
        {
            mnuManagerRooms_Click(sender, e);
        }

        private void đăngXuấtToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            BaseApiClient.ClearToken();
            Application.Restart();
        }

        private void thoátToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}