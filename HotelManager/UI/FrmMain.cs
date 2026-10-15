using HotelManagement.Desktop.Models;
using System;
using System.Windows.Forms;

namespace HotelManager.UI
{
    public partial class FrmMain : Form
    {
        private readonly UserSession _currentUser;

        public FrmMain(UserSession userSession)
        {
            InitializeComponent();
            _currentUser = userSession;
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            // Hiển thị thông tin người dùng lên tiêu đề form
            this.Text = $"Hệ thống Quản lý Khách sạn - [Xin chào: {_currentUser.Username} | Quyền: {_currentUser.Role}]";
        }

        // Sự kiện bấm vào menu Quản lý phòng
        // Sự kiện bấm vào menu Quản lý phòng
        private void mnuManageRooms_Click(object sender, EventArgs e)
        {
            // Kiểm tra nếu form đã mở thì active lên, tránh mở trùng lặp
            foreach (Form child in this.MdiChildren)
            {
                if (child is FrmManageRooms)
                {
                    child.Activate();
                    return;
                }
            }

            // Tạo mới form con
            FrmManageRooms frm = new FrmManageRooms();
            frm.MdiParent = this;
            frm.WindowState = FormWindowState.Maximized; // Phóng to toàn bộ vùng làm việc
            frm.Show();
        }

        // Sự kiện Đăng xuất hệ thống
        private void mnuLogout_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show("Bạn có muốn đăng xuất tài khoản không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                this.Hide();
                FrmLogin login = new FrmLogin();
                if (login.ShowDialog() == DialogResult.OK)
                {
                    // Nếu đăng nhập lại thành công thì mở lại form chính
                    this.Close();
                }
                else
                {
                    Application.Exit();
                }
            }
        }

        // Sự kiện Thoát ứng dụng
        private void mnuExit_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Bạn có chắc chắn muốn thoát chương trình không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}