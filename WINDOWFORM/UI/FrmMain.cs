using HotelManagement.Desktop.Models;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace HotelManager.UI
{
    public partial class FrmMain : Form
    {
        private readonly UserSession _currentUser;
        private Form? _dashboard;

        public FrmMain(UserSession userSession)
        {
            InitializeComponent();
            _currentUser = userSession;

            // Các mục này chưa được nối Click trong Designer gốc.
            mnuManageCustomers.Click += mnuManageCustomers_Click;
            mnuLogout.Click += mnuLogout_Click;
            mnuExit.Click += mnuExit_Click;
            mnuReports.Click += mnuReports_Click;

            // Bỏ thanh MenuStrip rỗng gây ra đường kẻ/thanh dư phía dưới menu.
            menuStrip1.Visible = false;
            MainMenuStrip = menuStrip2;

            var mnuHome = new ToolStripMenuItem("Tổng quan");
            mnuHome.Click += (s, e) => ShowDashboard();
            menuStrip2.Items.Insert(0, mnuHome);
            Shown += (s, e) => ShowDashboard();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            Text = $"Hệ thống Quản lý Khách sạn - [Xin chào: {_currentUser.Username} | Quyền: {_currentUser.Role}]";
        }

        private void ShowDashboard()
        {
            if (_dashboard != null && !_dashboard.IsDisposed)
            {
                _dashboard.Activate();
                return;
            }

            var home = new Form
            {
                Name = "FrmDashboardHome",
                Text = "Tổng quan",
                MdiParent = this,
                BackColor = Color.FromArgb(245, 247, 246),
                WindowState = FormWindowState.Maximized
            };
            _dashboard = home;

            var body = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(32),
                BackColor = home.BackColor,
                ColumnCount = 1,
                RowCount = 4,
                AutoScroll = true
            };
            body.RowStyles.Add(new RowStyle(SizeType.Absolute, 108));
            body.RowStyles.Add(new RowStyle(SizeType.Absolute, 102));
            body.RowStyles.Add(new RowStyle(SizeType.Absolute, 78));
            body.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            home.Controls.Add(body);

            var title = new Label
            {
                Dock = DockStyle.Fill,
                Text = "SAWADE  |  QUẢN LÝ KHÁCH SẠN\nTổng quan hệ thống",
                Font = new Font("Segoe UI", 21, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 61, 47),
                TextAlign = ContentAlignment.MiddleLeft
            };
            body.Controls.Add(title, 0, 0);

            var greeting = new Label
            {
                Dock = DockStyle.Fill,
                Text = $"Xin chào, {_currentUser.Username}  •  Quyền: {_currentUser.Role}\nChọn chức năng bên dưới để bắt đầu quản lý khách sạn.",
                Font = new Font("Segoe UI", 13),
                ForeColor = Color.FromArgb(80, 91, 85),
                TextAlign = ContentAlignment.MiddleLeft
            };
            body.Controls.Add(greeting, 0, 1);

            var actions = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                WrapContents = true
            };
            actions.Controls.Add(MakeButton("Quản lý phòng", (s, e) => OpenChild<FrmManageRooms>(() => new FrmManageRooms())));
            actions.Controls.Add(MakeButton("Quản lý khách hàng", (s, e) => OpenChild<FrmManageCustomers>(() => new FrmManageCustomers())));
            body.Controls.Add(actions, 0, 2);

            var notice = new Label
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                Padding = new Padding(0, 18, 0, 0),
                Text = "THÔNG TIN HỆ THỐNG\n\n" +
                       "• Quản lý phòng và khách hàng qua các mục trên thanh menu.\n\n" +
                       "• Số liệu thống kê sẽ xuất hiện khi nhóm hoàn thiện API và dữ liệu SQL Server.\n\n" +
                       "• Màn hình này chưa sử dụng số liệu giả lập.",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(55, 74, 64)
            };
            body.Controls.Add(notice, 0, 3);
            home.Show();
        }

        private static Button MakeButton(string caption, EventHandler handler)
        {
            var button = new Button
            {
                Text = caption,
                Width = 220,
                Height = 50,
                Margin = new Padding(0, 6, 16, 6),
                BackColor = Color.FromArgb(32, 65, 49),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            button.FlatAppearance.BorderSize = 0;
            button.Click += handler;
            return button;
        }

        private void OpenChild<T>(Func<T> factory) where T : Form
        {
            foreach (Form child in MdiChildren)
            {
                if (child is T)
                {
                    child.Activate();
                    child.WindowState = FormWindowState.Maximized;
                    return;
                }
            }
            T form = factory();
            form.MdiParent = this;
            form.WindowState = FormWindowState.Maximized;
            form.Show();
        }

        private void mnuManageRooms_Click(object sender, EventArgs e)
        {
            OpenChild(() => new FrmManageRooms());
        }

        private void mnuManageCustomers_Click(object? sender, EventArgs e)
        {
            OpenChild(() => new FrmManageCustomers());
        }

        private void mnuReports_Click(object? sender, EventArgs e)
        {
            MessageBox.Show("Báo cáo - Thống kê chưa được nối với API. Nhóm sẽ tích hợp sau khi có dữ liệu SQL Server.",
                "Thông tin", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void mnuLogout_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show("Bạn có muốn đăng xuất tài khoản không?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            Hide();
            using (var login = new FrmLogin())
            {
                if (login.ShowDialog() == DialogResult.OK)
                    Close();
                else
                    Application.Exit();
            }
        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn thoát chương trình không?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                Application.Exit();
        }
    }
}
