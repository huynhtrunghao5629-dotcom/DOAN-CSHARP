using HotelManagement.Desktop.Models;
using HotelManager.UI;
using System;
using System.Windows.Forms;

namespace HotelManager
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Bước 1: Mở màn hình đăng nhập trước
            FrmLogin loginForm = new FrmLogin();

            // Nếu người dùng đăng nhập thành công (trả về DialogResult.OK)
            if (loginForm.ShowDialog() == DialogResult.OK)
            {
                // Bước 2: Lấy thông tin UserSession truyền sang Form chính (FrmMain)
                UserSession? session = loginForm.CurrentUser;

                // Bước 3: Khởi chạy giao diện chính của hệ thống
                Application.Run(new FrmMain(session));
            }
        }
    }
}