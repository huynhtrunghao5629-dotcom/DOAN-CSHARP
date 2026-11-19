using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyKhachSan.Web.Models;

namespace QuanLyKhachSan.Web.Controllers;

[Authorize]
public class DonDatPhongController : Controller
{
    private readonly HttpClient _client;

    public DonDatPhongController(IHttpClientFactory factory)
    {
        _client = factory.CreateClient("HotelApi");
    }

    [HttpGet]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> ChiTiet(int id)
    {
        ViewData["NavbarOverlay"] = false;
        if (id <= 0) return NotFound();

        var phone = User.FindFirstValue(ClaimTypes.MobilePhone);
        var customerClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(phone) ||
            !int.TryParse(customerClaim, out var customerId) || customerId <= 0)
        {
            TempData["Loi"] = "Phiên đăng nhập không đầy đủ. Vui lòng đăng nhập lại.";
            return RedirectToAction("DangNhap", "TaiKhoan");
        }

        try
        {
            // Bước 1: chỉ cho xem các mã đơn trong lịch sử của SĐT đã xác thực.
            // Không nhận số điện thoại từ query string để tránh tra cứu chéo tài khoản.
            using var historyResponse = await _client.GetAsync(
                $"api/DatPhong/tra-cuu?sdt={Uri.EscapeDataString(phone)}");
            if (!historyResponse.IsSuccessStatusCode)
            {
                TempData["Loi"] = "Không thể kiểm tra quyền xem đơn lúc này. Vui lòng thử lại.";
                return RedirectToAction("HoSoCaNhan", "TaiKhoan");
            }

            var history = await historyResponse.Content.ReadFromJsonAsync<List<LichSuDatPhongViewModel>>() ?? new();
            var ownBooking = history.FirstOrDefault(x => x.DatPhongId == id);
            if (ownBooking == null) return NotFound();

            // Bước 2: lấy toàn bộ các phòng của đơn (API tra-cuu chỉ trả phòng đầu tiên).
            using var detailResponse = await _client.GetAsync($"api/DatPhong/{id}");
            if (detailResponse.StatusCode == HttpStatusCode.NotFound) return NotFound();
            if (!detailResponse.IsSuccessStatusCode)
            {
                TempData["Loi"] = "Chưa thể tải chi tiết đơn đặt phòng. Vui lòng thử lại.";
                return RedirectToAction("HoSoCaNhan", "TaiKhoan");
            }

            var booking = await detailResponse.Content.ReadFromJsonAsync<ChiTietDonViewModel>();
            if (booking == null || booking.DatPhongId != id ||
                booking.KhachHangId != customerId ||
                !string.Equals(booking.SoDienThoai, phone, StringComparison.Ordinal))
                return NotFound();

            booking.MaDon = string.IsNullOrWhiteSpace(ownBooking.MaDon)
                ? $"#DP-{id:D5}"
                : ownBooking.MaDon;
            return View(booking);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
        {
            TempData["Loi"] = "Kết nối API gián đoạn. Vui lòng thử xem chi tiết đơn sau.";
            return RedirectToAction("HoSoCaNhan", "TaiKhoan");
        }
    }
}
