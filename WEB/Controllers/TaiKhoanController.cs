using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using QuanLyKhachSan.Web.Models;
using System.Security.Claims;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace QuanLyKhachSan.Web.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly HttpClient _client;
        public TaiKhoanController(IHttpClientFactory factory) => _client = factory.CreateClient("HotelApi");
        public class ThongTinKhachHangDto
        {
            public string HoTen { get; set; } = "";
            public string SoDienThoai { get; set; } = "";
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
                    string apiToken = root.TryGetProperty("token", out var tokenElement)
                        ? tokenElement.GetString() ?? "" : "";
                    if (string.IsNullOrWhiteSpace(apiToken))
                    {
                        TempData["Loi"] = "Máy chủ chưa trả token đăng nhập. Vui lòng kiểm tra phiên bản API.";
                        return View(model);
                    }

                    // === CẤP COOKIE ĐĂNG NHẬP CHO WEB ===
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, khachHangId),
                        new Claim(ClaimTypes.Name, hoTen),
                        new Claim(ClaimTypes.MobilePhone, soDienThoai), // Số điện thoại thực từ API
                        new Claim("SawadeApiToken", apiToken) // Chỉ lưu trong cookie xác thực được bảo vệ
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
        [Authorize]
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
                var matchedOrder = history?.FirstOrDefault(d => d.DatPhongId == datPhongId);
                if (matchedOrder == null)
                {
                    TempData["Loi"] = "Không tìm thấy đơn đặt phòng thuộc tài khoản của bạn.";
                    return RedirectToAction("HoSoCaNhan");
                }
                if (!TrangThaiDatPhong.ChoPhepHuy(matchedOrder.TrangThai))
                {
                    TempData["Loi"] = "Chỉ có thể tự hủy đơn đang chờ xác nhận.";
                    return RedirectToAction("HoSoCaNhan");
                }
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
            {
                TempData["Loi"] = "Không thể kiểm tra đơn đặt phòng. Vui lòng thử lại.";
                return RedirectToAction("HoSoCaNhan");
            }
            try
            {
                var customerClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(customerClaim, out var khachHangId) || khachHangId <= 0)
                {
                    TempData["Loi"] = "Phiên đăng nhập không hợp lệ. Vui lòng đăng nhập lại.";
                    return RedirectToAction("DangNhap");
                }

                using var response = await client.PutAsJsonAsync(
                    $"api/DatPhong/{datPhongId}/huy-don",
                    new
                    {
                        KhachHangId = khachHangId,
                        SoDienThoai = phone
                    });
                if (response.IsSuccessStatusCode)
                    TempData["ThongBao"] = "Đã hủy yêu cầu đặt phòng.";
                else
                    TempData["Loi"] = "Không thể hủy đơn. Chỉ đơn đang chờ xác nhận mới được tự hủy.";
            }
            catch (HttpRequestException)
            {
                TempData["Loi"] = "Không thể kết nối API để hủy đơn. Vui lòng thử lại.";
            }
            catch (TaskCanceledException)
            {
                TempData["Loi"] = "API phản hồi quá lâu. Vui lòng kiểm tra trạng thái đơn trước khi thử lại.";
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
        [Authorize]
        [HttpGet]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> HoSoCaNhan()
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
                return RedirectToAction("DangNhap");

            var phoneClaim = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.MobilePhone);
            string sdt = phoneClaim?.Value ?? "";
            if (string.IsNullOrWhiteSpace(sdt))
            {
                await HttpContext.SignOutAsync("Cookies");
                TempData["Loi"] = "Phiên đăng nhập không còn hợp lệ. Vui lòng đăng nhập lại.";
                return RedirectToAction("DangNhap");
            }

            var donHangs = new List<QuanLyKhachSan.Web.Models.LichSuDatPhongViewModel>();
            ViewBag.LoiTaiLichSu = false;
            var client = _client;

            if (!string.IsNullOrEmpty(sdt))
            {
                try
                {
                    // Chỉ dùng số điện thoại có trong phiên đăng nhập, không nhận từ query string.
                    using var response = await client.GetAsync($"api/DatPhong/tra-cuu?sdt={Uri.EscapeDataString(sdt)}");
                    if (response.IsSuccessStatusCode)
                        donHangs = await response.Content.ReadFromJsonAsync<List<LichSuDatPhongViewModel>>() ?? new();
                    else
                    {
                        ViewBag.LoiTaiLichSu = true;
                        TempData["Loi"] = "Không thể tải lịch sử đặt phòng lúc này.";
                    }
                }
                catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
                {
                    ViewBag.LoiTaiLichSu = true;
                    TempData["Loi"] = "Kết nối đến API gián đoạn. Lịch sử đặt phòng chưa được cập nhật.";
                }
                try
                {
                    using var profileResponse = await client.GetAsync($"api/KhachHang/tim-kiem?tuKhoa={Uri.EscapeDataString(sdt)}");
                    if (profileResponse.IsSuccessStatusCode)
                    {
                        var khList = await profileResponse.Content.ReadFromJsonAsync<List<ThongTinKhachHangDto>>();
                        // Kết quả tìm kiếm có thể chứa nhiều khách; chỉ lấy khớp chính xác số điện thoại tài khoản.
                        var kh = khList?.FirstOrDefault(k =>
                            string.Equals(k.SoDienThoai?.Trim(), sdt, StringComparison.Ordinal));
                        if (kh != null)
                        {
                            ViewBag.Cccd = kh.Cccd;
                            ViewBag.Email = kh.Email;
                        }
                    }
                }
                catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
                {
                    // Chỉ phần email/CCCD bị thiếu, vẫn cho xem lịch sử đã tải.
                }
            }

            ViewBag.HoTen = User.Identity.Name;
            ViewBag.SoDienThoai = sdt;
            return View(donHangs);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatTaiKhoan(string hoTen, string cccd, string email, string matKhauMoi)
        {
            var phoneClaim = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.MobilePhone);
            string sdt = phoneClaim?.Value ?? "";
            if (string.IsNullOrEmpty(sdt)) return RedirectToAction("DangNhap");

            hoTen = hoTen?.Trim() ?? string.Empty;
            cccd = cccd?.Trim() ?? string.Empty;
            email = email?.Trim() ?? string.Empty;
            matKhauMoi ??= string.Empty;

            if (hoTen.Length is < 2 or > 120 || cccd.Length > 20 ||
                (!string.IsNullOrWhiteSpace(email) && !new EmailAddressAttribute().IsValid(email)) ||
                (!string.IsNullOrEmpty(matKhauMoi) && matKhauMoi.Length < 6))
            {
                TempData["Loi"] = "Thông tin chưa hợp lệ. Vui lòng kiểm tra họ tên, email và mật khẩu (ít nhất 6 ký tự nếu đổi).";
                return RedirectToAction("HoSoCaNhan");
            }

            var payload = new
            {
                SoDienThoai = sdt,
                HoTen = hoTen,
                CCCD = cccd,
                Email = email,
                MatKhauMoi = matKhauMoi
            };
            try
            {
                using var response = await _client.PutAsJsonAsync("api/KhachHang/cap-nhat-ho-so", payload);
                if (response.IsSuccessStatusCode)
                {
                    var claims = User.Claims.Where(c => c.Type != ClaimTypes.Name).ToList();
                    claims.Add(new Claim(ClaimTypes.Name, hoTen));
                    var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                    TempData["ThongBao"] = "Cập nhật hồ sơ thành công!";
                }
                else
                    TempData["Loi"] = "Cập nhật thất bại. Vui lòng kiểm tra thông tin hoặc thử lại.";
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                TempData["Loi"] = "Không thể kết nối API để cập nhật hồ sơ.";
            }

            return RedirectToAction("HoSoCaNhan");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangKy(DangKyViewModel model)
        {
            ViewData["NavbarOverlay"] = false;

            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                model.Email = model.Email.Trim();
                if (!Regex.IsMatch(model.Email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]{2,}$",
                    RegexOptions.CultureInvariant))
                {
                    ModelState.AddModelError(nameof(model.Email),
                        "Email phải có tên miền đầy đủ, ví dụ: example@gmail.com.");
                }
            }

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

                // Hiển thị chính xác thông báo lỗi API trả về thay vì câu báo cứng
                var apiError = await response.Content.ReadAsStringAsync();
                TempData["Loi"] = string.IsNullOrWhiteSpace(apiError)
                    ? $"Đăng ký thất bại (Mã lỗi {(int)response.StatusCode})."
                    : apiError.Trim('"');
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Loi"] = "Không thể kết nối đến Web API: " + ex.Message;
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