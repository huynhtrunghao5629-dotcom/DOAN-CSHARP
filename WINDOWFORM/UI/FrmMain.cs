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
            Text = $"Quản lý khách sạn - Tài khoản: {_user.Username} ({TrangThaiTiengViet.VaiTro(_user.Role)})";
            bool isAdmin = string.Equals(_user.Role, "Admin", StringComparison.OrdinalIgnoreCase);

            // Đồng bộ tên và phân quyền cho các menu tạo từ Designer
            mnuManageRooms.Text = "Quản lý phòng";
            mnuManageRooms.Visible = true;
            mnuManageRooms.Enabled = true;

            mnuReports.Visible = isAdmin;
            mnuReports.Enabled = isAdmin;

            mnuManageCustomers.Enabled = true;
            menuStrip1.Visible = false; // Menu trống do Designer cũ tạo.
            MainMenuStrip = menuStrip2;

            // Các chức năng nghiệp vụ quầy (Lễ tân và Quản lý đều dùng được)
            AddMenu("Quản lý đặt phòng", () => OpenChild(new FrmManageBookings()));
            AddMenu("Sơ đồ phòng", () => OpenChild(new FrmRoomGrid()));
            AddMenu("Dịch vụ phát sinh", () => OpenChild(new FrmManageServices()));
            AddMenu("Thanh toán / Trả phòng", () => OpenChild(new FrmManageInvoices()));

            // Các chức năng chỉ dành cho Quản lý (Admin)
            if (isAdmin)
            {
                AddMenu("Quản lý loại phòng / Giá", () => OpenChild(new FrmManageRoomTypes()));
                AddMenu("Quản lý tài khoản", () => OpenChild(new FrmManageUsers()));
                mnuReports.Click += (_, _) => OpenChild(new FrmReport());
            }

            // Gắn sự kiện cho các mục dùng chung
            mnuManageCustomers.Click += (_, _) => OpenChild(new FrmManageCustomers());
            mnuLogout.Click += (_, _) => { BaseApiClient.ClearToken(); Application.Restart(); };
            mnuExit.Click += (_, _) => Application.Exit();
        }

        private void AddMenu(string title, Action action)
        {
            var item = new ToolStripMenuItem(title);
            item.Click += (_, _) => action();
            menuStrip2.Items.Insert(menuStrip2.Items.Count - 1, item);
        }

        private void OpenChild(Form form)
        {
            foreach (Form child in MdiChildren)
            {
                if (child.GetType() == form.GetType())
                {
                    form.Dispose();
                    child.Activate();   
                    return;
                }
            }
            form.MdiParent = this;
            form.Show();
        }

        public void mnuManageRooms_Click(object sender, EventArgs e)
        {
            OpenChild(new FrmManageRooms(_user));
        }
    }
}