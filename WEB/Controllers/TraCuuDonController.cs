using Microsoft.AspNetCore.Mvc;
using QuanLyKhachSan.Web.Models;
using System.Text.Json;

namespace QuanLyKhachSan.Web.Controllers
{
    public class TraCuuDonController : Controller
    {
        private static readonly HttpClient _client = new()
        {
            BaseAddress = new Uri("https://localhost:7203/") // Đổi port nếu API của bạn khác
        };

        [HttpGet]
        public IActionResult Index()
        {
            ViewData["NavbarOverlay"] = false;
            // Nếu khách đã đăng nhập, tự động điền SĐT vào ô tìm kiếm
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var phoneClaim = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.MobilePhone);
                if (phoneClaim != null)
                {
                    ViewBag.SoDienThoai = phoneClaim.Value;
                }
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string soDienThoai)
        {
            ViewData["NavbarOverlay"] = false;
            ViewBag.SoDienThoai = soDienThoai;

            if (string.IsNullOrWhiteSpace(soDienThoai))
            {
                TempData["Loi"] = "Vui lòng nhập số điện thoại để tra cứu.";
                return View(new List<LichSuDatPhongViewModel>());
            }

            try
            {
                // Gọi thẳng xuống API để lấy dữ liệu thật
                var response = await _client.GetAsync($"api/DatPhong/tra-cuu?sdt={soDienThoai}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<List<LichSuDatPhongViewModel>>();
                    if (data == null || !data.Any())
                    {
                        TempData["Loi"] = $"Không tìm thấy đơn đặt phòng nào với số điện thoại {soDienThoai}.";
                    }
                    return View(data ?? new List<LichSuDatPhongViewModel>());
                }

                TempData["Loi"] = "Có lỗi xảy ra khi tra cứu dữ liệu.";
                return View(new List<LichSuDatPhongViewModel>());
            }
            catch
            {
                TempData["Loi"] = "Không thể kết nối đến máy chủ API.";
                return View(new List<LichSuDatPhongViewModel>());
            }
        }
    }
}