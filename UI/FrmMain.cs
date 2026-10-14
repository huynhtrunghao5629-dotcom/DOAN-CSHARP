using HotelManagement.Desktop.Models;
using System;
using System.Windows.Forms;
using HotelManagement.Desktop.UI;

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
            if (_currentUser != null)
            {
                this.Text = $"Hệ thống Quản lý Khách sạn - [Xin chào: {_currentUser.Username} | Quyền: {_currentUser.Role}]";
            }

            // Gọi hàm phân quyền khi form vừa mở
            ApplyRolePermissions();
        }

        private void ApplyRolePermissions()
        {
            if (_currentUser != null && "Staff".Equals(_currentUser.Role, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Đăng nhập với quyền Lễ tân (Staff). Một số tính năng quản trị sẽ bị giới hạn.",
                                "Thông báo phân quyền", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Sự kiện bấm vào menu Quản lý phòng
        private void mnuManageRooms_Click(object sender, EventArgs e)
        {
            if (_currentUser == null || !"Admin".Equals(_currentUser.Role, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Tài khoản của bạn không có quyền truy cập vào chức năng Quản lý phòng!", "Từ chối truy cập", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (Form child in this.MdiChildren)
            {
                if (child is FrmManageRooms)
                {
                    child.Activate();
                    return;
                }
            }

            FrmManageRooms frm = new FrmManageRooms(_currentUser);
            frm.MdiParent = this;
            frm.WindowState = FormWindowState.Maximized;
            frm.Show();
        }

        // Sự kiện bấm vào menu Quản lý khách hàng
        private void mnuManageCustomers_Click(object sender, EventArgs e)
        {
            bool hasAccess = _currentUser != null &&
                             ("Admin".Equals(_currentUser.Role, StringComparison.OrdinalIgnoreCase) ||
                              "Receptionist".Equals(_currentUser.Role, StringComparison.OrdinalIgnoreCase));

            if (!hasAccess)
            {
                MessageBox.Show("Tài khoản của bạn không có quyền truy cập vào chức năng Quản lý khách hàng!", "Từ chối truy cập", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (Form child in this.MdiChildren)
            {
                if (child is FrmManageCustomers)
                {
                    child.Activate();
                    return;
                }
            }

            FrmManageCustomers frm = new FrmManageCustomers(_currentUser);
            frm.MdiParent = this;
            frm.WindowState = FormWindowState.Maximized;
            frm.Show();
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
        }

        // Sự kiện bấm vào menu Quản lý đặt phòng
        private void mnuManageBookings_Click(object sender, EventArgs e)
        {
            foreach (Form child in this.MdiChildren)
            {
                if (child is FrmManageBookings)
                {
                    child.Activate();
                    return;
                }
            }

            FrmManageBookings frm = new FrmManageBookings(_currentUser);
            frm.MdiParent = this;
            frm.WindowState = FormWindowState.Maximized;
            frm.Show();
        }

        private void quảnLýĐặtPhòngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mnuManageBookings_Click(sender, e);
        }

        private void quảnLýHóaĐơnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form child in this.MdiChildren)
            {
                if (child is FrmManageInvoices)
                {
                    child.Activate();
                    return;
                }
            }

            FrmManageInvoices frm = new FrmManageInvoices(_currentUser);
            frm.MdiParent = this;
            frm.WindowState = FormWindowState.Maximized;
            frm.Show();
        }

        private void xemBáoCáoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form child in this.MdiChildren)
            {
                if (child is FrmReport)
                {
                    child.Activate();
                    return;
                }
            }

            FrmReport frm = new FrmReport(_currentUser);
            frm.MdiParent = this;
            frm.WindowState = FormWindowState.Maximized;
            frm.Show();
        }

        private void đặtPhòngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mnuManageBookings_Click(sender, e);
        }

        // Sự kiện Đăng xuất hệ thống (chuẩn)
        private void mnuLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn đăng xuất khỏi hệ thống?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                this.Hide();
                FrmLogin loginForm = new FrmLogin();
                loginForm.ShowDialog();
                this.Close();
            }
        }

        // Sự kiện Thoát ứng dụng (chuẩn)
        private void mnuExit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn thoát ứng dụng?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}