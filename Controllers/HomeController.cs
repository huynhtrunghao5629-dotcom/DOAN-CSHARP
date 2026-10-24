using Microsoft.AspNetCore.Mvc;
using QuanLyKhachSan.Web.Models;
using System.Collections.Generic;

namespace QuanLyKhachSan.Web.Controllers
{
    public class HomeController : Controller
    {
        // Trang chủ
        public IActionResult Index()
        {
            return View();
        }

        // Trang Danh mục phòng
        public IActionResult DanhSachPhong()
        {
            // Tạo dữ liệu giả lập sử dụng đúng các thuộc tính trong PhongViewModel của bạn
            var danhSach = new List<PhongViewModel>
            {
                new PhongViewModel { PhongId = "P1", SoPhong = "101", TenLoaiPhong = "Deluxe", GiaPhong = 1200000, TrangThai = "Trống" },
                new PhongViewModel { PhongId = "P2", SoPhong = "102", TenLoaiPhong = "Standard", GiaPhong = 800000, TrangThai = "Trống" },
                new PhongViewModel { PhongId = "P3", SoPhong = "201", TenLoaiPhong = "Suite", GiaPhong = 3500000, TrangThai = "Đã đặt" },
                new PhongViewModel { PhongId = "P4", SoPhong = "202", TenLoaiPhong = "Deluxe", GiaPhong = 1200000, TrangThai = "Trống" }
            };

            // Truyền danh sách này ra View
            return View(danhSach);
        }

        // Trang Chi tiết phòng (Nhận ID phòng để hiển thị chi tiết)
        public IActionResult ChiTietPhong(string id)
        {
            // Tạm thời giả lập việc lấy 1 phòng theo ID
            var phong = new PhongViewModel { PhongId = id, SoPhong = "101", TenLoaiPhong = "Deluxe", GiaPhong = 1200000, TrangThai = "Trống" };
            return View(phong);
        }
    }
}