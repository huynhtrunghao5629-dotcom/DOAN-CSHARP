using HotelManagement.Desktop.Models;               
using System;
using System.Windows.Forms;

namespace HotelManager.UI
{
    public partial class FrmLogin : Form
    {
        // Thuộc tính lưu thông tin phiên đăng nhập sau khi xác thực thành công
        public UserSession? CurrentUser { get; private set; }

        public FrmLogin()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            // Kiểm tra ràng buộc nhập liệu cơ bản
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Giả lập tài khoản xác thực & phân quyền (Admin / Staff)
            if (username == "admin" && password == "123456")
            {
                CurrentUser = new UserSession { Username = username, Role = "Admin", Token = "fake-jwt-token-admin" };
                MessageBox.Show("Đăng nhập thành công với quyền Admin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else if (username == "staff" && password == "123456")
            {
                CurrentUser = new UserSession { Username = username, Role = "Staff", Token = "fake-jwt-token-staff" };
                MessageBox.Show("Đăng nhập thành công với quyền Staff!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Tài khoản hoặc mật khẩu không chính xác!", "Lỗi xác thực", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPassword.Clear();
                txtPassword.Focus();
            }
        }
    }
}