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
        private readonly HttpClient _client;
        public TaiKhoanController(IHttpClientFactory factory) => _client = factory.CreateClient("HotelApi");
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
        [ValidateAntiForgeryToken]
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
                    string soDienThoai = root.GetProperty("soDienThoai").GetString() ?? "";

                    // === CẤP COOKIE ĐĂNG NHẬP CHO WEB ===
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, khachHangId),
                        new Claim(ClaimTypes.Name, hoTen),
                        new Claim(ClaimTypes.MobilePhone, soDienThoai) // Số điện thoại thực từ API, không dùng input email/CCCD
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuyDonHang(int datPhongId)
        {
            // Phải đăng nhập mới được hủy
            if (User.Identity == null || !User.Identity.IsAuthenticated)
                return RedirectToAction("DangNhap");

            if (datPhongId <= 0) return BadRequest();
            var phone = User.FindFirst(ClaimTypes.MobilePhone)?.Value;
            if (string.IsNullOrWhiteSpace(phone)) return Forbid();
            var client = _client;
            try
            {
                var history = await client.GetFromJsonAsync<List<LichSuDatPhongViewModel>>(
                    $"api/DatPhong/tra-cuu?sdt={Uri.EscapeDataString(phone)}");
                if (history == null || !history.Any(d => d.DatPhongId == datPhongId))
                {
                    TempData["Loi"] = "Không tìm thấy đơn đặt phòng thuộc tài khoản của bạn.";
                    return RedirectToAction("HoSoCaNhan");
                }
            }
            catch (HttpRequestException)
            {
                TempData["Loi"] = "Không thể kiểm tra đơn đặt phòng. Vui lòng thử lại.";
                return RedirectToAction("HoSoCaNhan");
            }
            using var response = await client.PutAsync($"api/DatPhong/{datPhongId}/huy-don", null);

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
            var client = _client;

            if (!string.IsNullOrEmpty(sdt))
            {
                // 1. Lấy lịch sử đặt phòng
                var response = await client.GetAsync($"api/DatPhong/tra-cuu?sdt={Uri.EscapeDataString(sdt)}");
                if (response.IsSuccessStatusCode)
                    donHangs = await response.Content.ReadFromJsonAsync<List<QuanLyKhachSan.Web.Models.LichSuDatPhongViewModel>>() ?? new();

                // 2. Lấy thông tin cá nhân (CCCD, Email) để điền sẵn vào Form sửa
                var profileResponse = await client.GetAsync($"api/KhachHang/tim-kiem?tuKhoa={Uri.EscapeDataString(sdt)}");
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
        [ValidateAntiForgeryToken]
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
            var client = _client;
            var response = await client.PutAsJsonAsync("api/KhachHang/cap-nhat-ho-so", payload);

            if (response.IsSuccessStatusCode)
            {
                // Ghi đè lại Cookie ngay lập tức với Họ tên mới
                // Preserve ID and all existing claims; change only the display name.
                var claims = User.Claims.Where(c => c.Type != ClaimTypes.Name).ToList();
                claims.Add(new Claim(ClaimTypes.Name, hoTen ?? string.Empty));

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
        [ValidateAntiForgeryToken]
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangXuat()
        {
            // Xóa Cookie Đăng xuất
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToAction("Index", "Home");
        }
    }
}