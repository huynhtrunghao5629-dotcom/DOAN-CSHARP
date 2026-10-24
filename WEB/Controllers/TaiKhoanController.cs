using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using QuanLyKhachSan.Web.Models;
using System.Security.Claims;
using System.Text.Json;

namespace QuanLyKhachSan.Web.Controllers
{
    public class TaiKhoanController : Controller
    {
        // Cấu hình HttpClient gọi qua Web API (Đổi port 7203 cho đúng với API của bạn nếu khác)
        private static readonly HttpClient _client = new()
        {
            BaseAddress = new Uri("https://localhost:7203/")
        };
        public class ThongTinKhachHangDto
        {
            public string HoTen { get; set; } = "";
            public string Cccd { get; set; } = "";
            public string Email { get; set; } = "";
        }
        [HttpGet]
        public IActionResult DangNhap()
        {
            ViewData["NavbarOverlay"] = false;
            // Nếu đã đăng nhập rồi thì đá về trang chủ
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DangNhap(DangNhapViewModel model)
        {
            ViewData["NavbarOverlay"] = false;
            if (!ModelState.IsValid) return View(model);

            try
            {
                // Gửi request lên Web API để kiểm tra đăng nhập
                var response = await _client.PostAsJsonAsync("api/KhachHang/dang-nhap", new
                {
                    TaiKhoan = model.TaiKhoan.Trim(),
                    MatKhau = model.MatKhau
                });

                if (response.IsSuccessStatusCode)
                {
                    var userJson = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(userJson);
                    var root = doc.RootElement;

                    // Đọc thông tin trả về từ API
                    string khachHangId = root.GetProperty("khachHangId").ToString();
                    string hoTen = root.GetProperty("hoTen").GetString() ?? "Khách hàng";

                    // === CẤP COOKIE ĐĂNG NHẬP CHO WEB ===
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, khachHangId),
                        new Claim(ClaimTypes.Name, hoTen),
                        new Claim(ClaimTypes.MobilePhone, model.TaiKhoan) // Số điện thoại
                    };
                    var identity = new ClaimsIdentity(claims, "Cookies");
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync("Cookies", principal);
                    TempData["ThongBao"] = $"Chào mừng {hoTen} trở lại!";
                    return RedirectToAction("Index", "Home");
                }

                TempData["Loi"] = "Số điện thoại hoặc mật khẩu không chính xác.";
                return View(model);
            }
            catch
            {
                TempData["Loi"] = "Lỗi kết nối đến máy chủ API. Hãy chắc chắn API đang chạy.";
                return View(model);
            }
        }
        [HttpPost]
        public async Task<IActionResult> HuyDonHang(int datPhongId)
        {
            // Phải đăng nhập mới được hủy
            if (User.Identity == null || !User.Identity.IsAuthenticated)
                return RedirectToAction("DangNhap");

            using var client = new HttpClient { BaseAddress = new Uri("https://localhost:7203/") }; // Đổi port API cho đúng máy bạn
            var response = await client.PutAsync($"api/DatPhong/{datPhongId}/huy-don", null);

            if (response.IsSuccessStatusCode)
            {
                TempData["ThongBao"] = "Hủy đơn đặt phòng thành công!";
            }
            else
            {
                TempData["Loi"] = "Hủy đơn thất bại. Đơn này có thể đã được xử lý bởi lễ tân.";
            }

            return RedirectToAction("HoSoCaNhan");
        }
        [HttpGet]
        public IActionResult DangKy()
        {
            ViewData["NavbarOverlay"] = false;
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> HoSoCaNhan()
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
                return RedirectToAction("DangNhap");

            var phoneClaim = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.MobilePhone);
            string sdt = phoneClaim?.Value ?? "";

            var donHangs = new List<QuanLyKhachSan.Web.Models.LichSuDatPhongViewModel>();
            using var client = new HttpClient { BaseAddress = new Uri("https://localhost:7203/") }; // Sửa lại port API nếu cần

            if (!string.IsNullOrEmpty(sdt))
            {
                // 1. Lấy lịch sử đặt phòng
                var response = await client.GetAsync($"api/DatPhong/tra-cuu?sdt={sdt}");
                if (response.IsSuccessStatusCode)
                    donHangs = await response.Content.ReadFromJsonAsync<List<QuanLyKhachSan.Web.Models.LichSuDatPhongViewModel>>() ?? new();

                // 2. Lấy thông tin cá nhân (CCCD, Email) để điền sẵn vào Form sửa
                var profileResponse = await client.GetAsync($"api/KhachHang/tim-kiem?tuKhoa={sdt}");
                if (profileResponse.IsSuccessStatusCode)
                {
                    var khList = await profileResponse.Content.ReadFromJsonAsync<List<ThongTinKhachHangDto>>();
                    var kh = khList?.FirstOrDefault();
                    if (kh != null)
                    {
                        ViewBag.Cccd = kh.Cccd;
                        ViewBag.Email = kh.Email;
                    }
                }
            }

            ViewBag.HoTen = User.Identity.Name;
            ViewBag.SoDienThoai = sdt;
            return View(donHangs);
        }

        [HttpPost]
        public async Task<IActionResult> CapNhatTaiKhoan(string hoTen, string cccd, string email, string matKhauMoi)
        {
            var phoneClaim = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.MobilePhone);
            string sdt = phoneClaim?.Value ?? "";
            if (string.IsNullOrEmpty(sdt)) return RedirectToAction("DangNhap");

            var payload = new
            {
                SoDienThoai = sdt,
                HoTen = hoTen,
                CCCD = cccd,
                Email = email,
                MatKhauMoi = matKhauMoi
            };
            using var client = new HttpClient { BaseAddress = new Uri("https://localhost:7203/") };
            var response = await client.PutAsJsonAsync("api/KhachHang/cap-nhat-ho-so", payload);

            if (response.IsSuccessStatusCode)
            {
                // Ghi đè lại Cookie ngay lập tức với Họ tên mới
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, hoTen), // Tên mới cập nhật
            new Claim(ClaimTypes.MobilePhone, sdt) // SĐT giữ nguyên
        };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                TempData["ThongBao"] = "Cập nhật hồ sơ thành công!";
            }
            else
            {
                TempData["Loi"] = "Cập nhật thất bại. Số CCCD có thể đã bị trùng lặp.";
            }

            return RedirectToAction("HoSoCaNhan");
        }
            [HttpPost]
        public async Task<IActionResult> DangKy(DangKyViewModel model)
        {
            ViewData["NavbarOverlay"] = false;
            if (!ModelState.IsValid) return View(model);

            try
            {
                // Gọi API Đăng ký
                var response = await _client.PostAsJsonAsync("api/KhachHang/dang-ky", new
                {
                    HoTen = model.HoTen.Trim(),
                    SoDienThoai = model.SoDienThoai.Trim(),
                    CCCD = model.CCCD.Trim(),
                    Email = model.Email?.Trim(),
                    MatKhau = model.MatKhau
                });

                if (response.IsSuccessStatusCode)
                {
                    TempData["ThongBao"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                    return RedirectToAction("DangNhap");
                }

                var error = await response.Content.ReadAsStringAsync();
                TempData["Loi"] = $"Đăng ký thất bại: {error}";
                return View(model);
            }
            catch
            {
                TempData["Loi"] = "Không thể kết nối đến Web API.";
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> DangXuat()
        {
            // Xóa Cookie Đăng xuất
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToAction("Index", "Home");
        }
    }
}