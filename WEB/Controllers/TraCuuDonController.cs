using Microsoft.AspNetCore.Mvc;
using QuanLyKhachSan.Web.Models;
using System.Text.Json;

namespace QuanLyKhachSan.Web.Controllers
{
    public class TraCuuDonController : Controller
    {
        private readonly HttpClient _client;
        public TraCuuDonController(IHttpClientFactory factory) => _client = factory.CreateClient("HotelApi");

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (User.Identity?.IsAuthenticated != true) return RedirectToAction("DangNhap", "TaiKhoan");
            var phone = User.FindFirst(System.Security.Claims.ClaimTypes.MobilePhone)?.Value;
            if (string.IsNullOrWhiteSpace(phone)) return Forbid();
            ViewData["NavbarOverlay"] = false;
            ViewBag.SoDienThoai = phone;
            try
            {
                using var response = await _client.GetAsync($"api/DatPhong/tra-cuu?sdt={Uri.EscapeDataString(phone)}");
                if (response.IsSuccessStatusCode)
                    return View(await response.Content.ReadFromJsonAsync<List<LichSuDatPhongViewModel>>() ?? new());
                TempData["Loi"] = "Không thể lấy lịch sử đặt phòng lúc này.";
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
            {
                TempData["Loi"] = "Không thể kết nối API để tra cứu đơn.";
            }
            return View(new List<LichSuDatPhongViewModel>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string soDienThoai)
        {
            if (User.Identity?.IsAuthenticated != true) return RedirectToAction("DangNhap", "TaiKhoan");
            ViewData["NavbarOverlay"] = false;
            var currentPhone = User.FindFirst(System.Security.Claims.ClaimTypes.MobilePhone)?.Value;
            if (string.IsNullOrEmpty(currentPhone)) return Forbid();
            if (!string.Equals(soDienThoai?.Trim(), currentPhone, StringComparison.Ordinal))
            {
                TempData["Loi"] = "Chỉ có thể xem lịch sử đặt phòng của tài khoản đang đăng nhập.";
                return RedirectToAction(nameof(Index));
            }
            soDienThoai = currentPhone;
            ViewBag.SoDienThoai = soDienThoai;

            if (string.IsNullOrWhiteSpace(soDienThoai))
            {
                TempData["Loi"] = "Vui lòng nhập số điện thoại để tra cứu.";
                return View(new List<LichSuDatPhongViewModel>());
            }

            try
            {
                // Gọi thẳng xuống API để lấy dữ liệu thật
                var response = await _client.GetAsync($"api/DatPhong/tra-cuu?sdt={Uri.EscapeDataString(soDienThoai)}");
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