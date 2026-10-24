<<<<<<< HEAD
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
=======
namespace QuanLyKhachSan.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
>>>>>>> a12b9eb3352c71545027301255a333bcc94aa313
