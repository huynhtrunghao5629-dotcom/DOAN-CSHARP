using HotelManagement.Desktop.Models;
using HotelManager.Services;
using System;
using System.Windows.Forms;

namespace HotelManager.UI
{
    public partial class FrmMain : Form
    {
        public static UserSession? CurrentSession { get; private set; }

        public FrmMain(UserSession userSession)
        {
            InitializeComponent();
            CurrentSession = userSession;
            ConfigureNavigation();
        }

        public FrmMain()
        {
            InitializeComponent();
            ConfigureNavigation();
        }

        // Nối sự kiện tại đây để không phải chỉnh file Designer tự sinh.
        // Các menu mới chỉ mở form; API/SQL vẫn do từng form phụ trách.
        private void ConfigureNavigation()
        {
            mnuManageCustomers.Click += mnuManageCustomers_Click;
            mnuReports.Click += mnuReport_Click;
            mnuLogout.Click += mnuLogout_Click;
            mnuExit.Click += mnuExit_Click;

            menuStrip2.Items.Insert(2, CreateMenu("Đặt phòng", mnuManageBookings_Click));
            menuStrip2.Items.Insert(3, CreateMenu("Sơ đồ phòng", mnuRoomGrid_Click));
            menuStrip2.Items.Insert(4, CreateMenu("Dịch vụ", mnuManageServices_Click));
            menuStrip2.Items.Insert(5, CreateMenu("Hóa đơn", mnuManageInvoices_Click));

            hệThốngToolStripMenuItem.DropDownItems.Insert(0,
                CreateMenu("Quản lý nhân viên", mnuManageUsers_Click));
            hệThốngToolStripMenuItem.DropDownItems.Insert(1, new ToolStripSeparator());

            // Designer tạo một thanh menu rỗng. Chỉ hiển thị thanh menu thực tế.
            menuStrip1.Visible = false;
            MainMenuStrip = menuStrip2;
        }

        private static ToolStripMenuItem CreateMenu(string title, EventHandler click)
        {
            var menu = new ToolStripMenuItem(title);
            menu.Click += click;
            return menu;
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            if (CurrentSession != null)
            {
                this.Text = $"Hệ thống Quản lý Khách sạn - [Xin chào: {CurrentSession.Username} | Quyền: {TrangThaiTiengViet.VaiTro(CurrentSession.Role)}]";
            }
        }

        private void OpenChildForm<T>() where T : Form, new()
        {
            foreach (Form child in this.MdiChildren)
            {
                if (child is T)
                {
                    child.Activate();
                    return;
                }
            }

            T frm = new T
            {
                MdiParent = this,
                WindowState = FormWindowState.Maximized
            };
            frm.Show();
        }

        // Quản lý Phòng (Đã có Form)
        private void mnuManageRooms_Click(object sender, EventArgs e)
        {
            OpenChildForm<FrmManageRooms>();
        }

        // Quản lý Khách hàng (Đã có Form)
        private void mnuManageCustomers_Click(object sender, EventArgs e)
        {
            OpenChildForm<FrmManageCustomers>();
        }

        // Các chức năng đang phát triển (Tạm thời hiện thông báo để không bị lỗi Build)
        private void mnuManageBookings_Click(object sender, EventArgs e)
        {
            OpenChildForm<FrmManageBookings>();
        }

        private void mnuManageRoomTypes_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng đang được phát triển!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void mnuLogout_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show("Bạn có muốn đăng xuất không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                CurrentSession = null;
                this.Hide();
                FrmLogin login = new FrmLogin();
                if (login.ShowDialog() == DialogResult.OK)
                {
                    this.Close();
                }
                else
                {
                    Application.Exit();
                }
            }
        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Thoát chương trình?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        // Sự kiện Click menu Sơ đồ phòng
        private void mnuRoomGrid_Click(object sender, EventArgs e)
        {
            OpenChildForm<FrmRoomGrid>();
        }

        // Sự kiện Click menu Quản lý / Ghi nhận Dịch vụ
        private void mnuManageServices_Click(object sender, EventArgs e)
        {
            OpenChildForm<FrmManageServices>();
        }

        // Sự kiện Click menu Quản lý Hóa đơn & Thanh toán
        private void mnuManageInvoices_Click(object sender, EventArgs e)
        {
            OpenChildForm<FrmManageInvoices>();
        }

        // Sự kiện Click menu Quản lý Tài khoản Nhân viên
        private void mnuManageUsers_Click(object sender, EventArgs e)
        {
            // Chỉ tài khoản Role Admin mới được phép mở màn hình quản lý nhân viên
            if (CurrentSession != null && CurrentSession.Role != "Admin")
            {
                MessageBox.Show("Chức năng này chỉ dành riêng cho Quản trị viên (Admin)!", "Truy cập bị từ chối", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            OpenChildForm<FrmManageUsers>();
        }

        private void mnuReport_Click(object sender, EventArgs e)
        {
            OpenChildForm<FrmReport>();
        }
    }
}