using System;
using System.Windows.Forms;
using HotelManagement.Desktop.Models;
using HotelManager.UI;

namespace HotelManager
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 1. Mở màn hình đăng nhập FrmLogin trước
            FrmLogin loginForm = new FrmLogin();

            // 2. Nếu đăng nhập thành công, lấy thông tin UserSession chuyển sang FrmMain
            if (loginForm.ShowDialog() == DialogResult.OK && loginForm.CurrentUser != null)
            {
                Application.Run(new FrmMain(loginForm.CurrentUser));
            }
        }
    }
}