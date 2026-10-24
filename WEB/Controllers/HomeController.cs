using Microsoft.AspNetCore.Mvc;
using QuanLyKhachSan.Web.Models;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace QuanLyKhachSan.Web.Controllers
{
    public class HomeController : Controller
    {
        // Giữ cùng địa chỉ API mà dự án hiện tại đang sử dụng.
        // Nếu API chạy ở cổng khác, hãy đổi BaseAddress này cho đúng với launchSettings.json của API.
        private static readonly HttpClient _client = new()
        {
            BaseAddress = new Uri("https://localhost:7203/")
        };

        public async Task<IActionResult> Index()
        {
            ViewData["NavbarOverlay"] = true;

            try
            {
                var rooms = await GetRoomsAsync("api/Phong");
                return View(rooms);
            }
            catch (Exception)
            {
                ViewBag.ApiError = "Không thể tải dữ liệu phòng. Vui lòng kiểm tra Web API đang chạy và địa chỉ API trong HomeController.";
                return View(new List<PhongViewModel>());
            }
        }

        public async Task<IActionResult> DanhSachPhong(
    int? loaiPhong,
    decimal? giaTu,
    decimal? giaDen,
    DateOnly? ngayNhan,
    DateOnly? ngayTra,
    string? sapXepGia,
    bool chiPhongTrong = false,
    int trang = 1)
        {
            ViewData["NavbarOverlay"] = false;
            ViewBag.LoaiPhong = loaiPhong;
            ViewBag.GiaTu = giaTu;
            ViewBag.GiaDen = giaDen;
            ViewBag.NgayNhan = ngayNhan;
            ViewBag.NgayTra = ngayTra;
            ViewBag.SapXepGia = sapXepGia;
            ViewBag.ChiPhongTrong = chiPhongTrong;

            try
            {
                // 1. Khởi tạo danh sách phòng rỗng để tránh lỗi unassigned local variable
                List<PhongViewModel> rooms = new List<PhongViewModel>();
                bool coNgayNhan = ngayNhan.HasValue;
                bool coNgayTra = ngayTra.HasValue;

                // 2. Logic kiểm tra và gọi API theo Ngày nhận/trả
                if (coNgayNhan != coNgayTra)
                {
                    ViewBag.FilterError = "Vui lòng chọn cả ngày nhận phòng và ngày trả phòng.";
                    rooms = await GetRoomsAsync("api/Phong");
                }
                else if (coNgayNhan && coNgayTra)
                {
                    if (ngayNhan!.Value < DateOnly.FromDateTime(DateTime.Today) || ngayTra!.Value <= ngayNhan.Value)
                    {
                        ViewBag.FilterError = "Ngày nhận phòng phải từ hôm nay trở đi và ngày trả phòng phải sau ngày nhận phòng.";
                        rooms = await GetRoomsAsync("api/Phong");
                    }
                    else
                    {
                        var query = $"api/Phong/tim-kiem?ngayNhan={ngayNhan.Value.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture)}&ngayTra={ngayTra.Value.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture)}";
                        rooms = await GetRoomsAsync(query);
                    }
                }
                else
                {
                    rooms = await GetRoomsAsync("api/Phong");
                }

                // 3. Lọc theo checkbox "Chỉ hiển thị phòng trống"
                if (chiPhongTrong)
                {
                    rooms = rooms.Where(p =>
                        (p.TrangThai ?? "").Trim().ToLowerInvariant() == "trong" ||
                        (p.TrangThai ?? "").Trim().ToLowerInvariant() == "trống").ToList();
                }

                // 4. Lọc theo Hạng phòng
                if (loaiPhong.HasValue && loaiPhong.Value > 0)
                {
                    rooms = rooms.Where(p => p.LoaiPhongId == loaiPhong.Value).ToList();
                }

                // 5. Lọc theo Khoảng giá (Từ - Đến)
                if (giaTu.HasValue && giaTu.Value >= 0)
                {
                    rooms = rooms.Where(p => p.GiaPhong >= giaTu.Value).ToList();
                }
                if (giaDen.HasValue && giaDen.Value > 0)
                {
                    rooms = rooms.Where(p => p.GiaPhong <= giaDen.Value).ToList();
                }

                // 6. Sắp xếp giá Tăng/Giảm
                if (!string.IsNullOrEmpty(sapXepGia))
                {
                    if (sapXepGia == "tang")
                        rooms = rooms.OrderBy(p => p.GiaPhong).ToList();
                    else if (sapXepGia == "giam")
                        rooms = rooms.OrderByDescending(p => p.GiaPhong).ToList();
                }

                // 7. Logic Phân trang (Cắt dữ liệu mang ra View)
                int soPhongMoiTrang = 6;
                int tongSoPhong = rooms.Count;
                int tongSoTrang = (int)Math.Ceiling((double)tongSoPhong / soPhongMoiTrang);

                if (trang < 1) trang = 1;
                if (tongSoTrang > 0 && trang > tongSoTrang) trang = tongSoTrang;

                var phongTrenTrang = rooms
                    .Skip((trang - 1) * soPhongMoiTrang)
                    .Take(soPhongMoiTrang)
                    .ToList();

                ViewBag.TrangHienTai = trang;
                ViewBag.TongSoTrang = tongSoTrang;

                return View(phongTrenTrang);
            }
            catch (Exception)
            {
                ViewBag.ApiError = "Không thể tải danh sách phòng. Vui lòng kiểm tra kết nối Web API.";
                return View(new List<PhongViewModel>());
            }
        }

        public async Task<IActionResult> ChiTietPhong(int id)
        {
            if (id <= 0)
                return BadRequest("Mã phòng không hợp lệ.");

            try
            {
                using var response = await _client.GetAsync($"api/Phong/{id}");
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return NotFound();

                response.EnsureSuccessStatusCode();
                var room = await response.Content.ReadFromJsonAsync<PhongViewModel>();
                if (room == null)
                    return NotFound();

                ViewData["NavbarOverlay"] = false;
                return View(room);
            }
            catch (HttpRequestException)
            {
                return StatusCode(503, "Không thể kết nối Web API để tải chi tiết phòng.");
            }
            catch (JsonException)
            {
                return StatusCode(502, "Dữ liệu chi tiết phòng từ Web API không đúng định dạng.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DatPhong(
    int PhongId,
    string? HoTen,
    string? SoDienThoai,
    string? CCCD,
    DateTime? NgayNhan,
    DateTime? NgayTra)
        {
            IActionResult QuayLaiChiTiet() => RedirectToAction(nameof(ChiTietPhong), new { id = PhongId })!;

            if (PhongId <= 0)
            {
                TempData["Loi"] = "Mã phòng không hợp lệ.";
                return RedirectToAction(nameof(DanhSachPhong));
            }

            if (string.IsNullOrWhiteSpace(HoTen) || string.IsNullOrWhiteSpace(CCCD))
            {
                TempData["Loi"] = "Vui lòng nhập đầy đủ họ tên và CCCD/CMND.";
                return QuayLaiChiTiet();
            }

            if (string.IsNullOrWhiteSpace(SoDienThoai) || !Regex.IsMatch(SoDienThoai.Trim(), @"^\d{10,11}$"))
            {
                TempData["Loi"] = "Số điện thoại phải gồm 10–11 chữ số.";
                return QuayLaiChiTiet();
            }

            if (!NgayNhan.HasValue || !NgayTra.HasValue ||
                    NgayNhan.Value < DateTime.Now.AddMinutes(-10) || // Cho phép trễ 10 phút do thao tác chậm
                    NgayTra.Value <= NgayNhan.Value)
            {
                TempData["Loi"] = "Thời gian nhận phòng không hợp lệ hoặc thời gian trả phòng phải sau thời gian nhận phòng.";
                return QuayLaiChiTiet();
            }

            try
            {
                // === BƯỚC 1: KIỂM TRA TRẠNG THÁI HIỆN TẠI CỦA PHÒNG ===
                using var roomResponse = await _client.GetAsync($"api/Phong/{PhongId}");
                if (roomResponse.IsSuccessStatusCode)
                {
                    var room = await roomResponse.Content.ReadFromJsonAsync<PhongViewModel>();
                    if (room != null)
                    {
                        var status = room.TrangThai.Trim().ToLowerInvariant();
                        // Chặn ngay nếu phòng đang có khách, bảo trì, hoặc đã đặt
                        if (status.Contains("khách") || status.Contains("khach") ||
                            status.Contains("bảo trì") || status.Contains("bao tri") ||
                            status.Contains("đặt") || status.Contains("dat") || status == "occupied")
                        {
                            TempData["Loi"] = $"Lỗi: Không thể đặt! Phòng này hiện '{room.TrangThai}'.";
                            return QuayLaiChiTiet();
                        }
                    }
                }

                // === BƯỚC 2: KIỂM TRA TRÙNG LỊCH TRONG TƯƠNG LAI ===
                var ngayNhan = DateOnly.FromDateTime(NgayNhan.Value.Date);
                var ngayTra = DateOnly.FromDateTime(NgayTra.Value.Date);
                var availabilityUrl = $"api/Phong/tim-kiem?ngayNhan={ngayNhan:yyyy-MM-dd}&ngayTra={ngayTra:yyyy-MM-dd}";
                var availableRooms = await GetRoomsAsync(availabilityUrl);

                if (!availableRooms.Any(p => p.PhongId == PhongId))
                {
                    TempData["Loi"] = "Phòng này đã có người đặt trong khoảng thời gian bạn chọn. Vui lòng chọn ngày khác.";
                    return QuayLaiChiTiet();
                }

                // === BƯỚC 3: TIẾN HÀNH LƯU KHÁCH HÀNG VÀ ĐẶT PHÒNG ===
                var customerPayload = new
                {
                    HoTen = HoTen.Trim(),
                    SoDienThoai = SoDienThoai.Trim(),
                    CCCD = CCCD.Trim()
                };

                using var customerResponse = await _client.PostAsJsonAsync("api/KhachHang", customerPayload);
                var customerBody = await customerResponse.Content.ReadAsStringAsync();

                if (!customerResponse.IsSuccessStatusCode)
                {
                    TempData["Loi"] = "Không thể lưu thông tin khách hàng: " + FormatApiError(customerResponse.StatusCode, customerBody);
                    return QuayLaiChiTiet();
                }

                int? khachHangId = TryReadCustomerId(customerBody);
                if (!khachHangId.HasValue || khachHangId.Value <= 0)
                {
                    TempData["Loi"] = "Lỗi hệ thống: Web API không trả về mã khách hàng.";
                    return QuayLaiChiTiet();
                }

                var bookingPayload = new
                {
                    KhachHangId = khachHangId.Value,
                    NgayNhan = NgayNhan.Value, 
                    NgayTra = NgayTra.Value,   
                    DanhSachPhongId = new List<int> { PhongId }
                };

                using var bookingResponse = await _client.PostAsJsonAsync("api/DatPhong", bookingPayload);
                if (bookingResponse.IsSuccessStatusCode)
                {
                    TempData["ThongBao"] = "🎉 Đặt phòng thành công! Đơn hàng đã được ghi nhận vào hệ thống với thời gian chính xác.";
                    return RedirectToAction(nameof(Index));
                }

                var bookingError = await bookingResponse.Content.ReadAsStringAsync();
                TempData["Loi"] = "Không thể đặt phòng: " + FormatApiError(bookingResponse.StatusCode, bookingError);
                return QuayLaiChiTiet();
            }
            catch (HttpRequestException)
            {
                TempData["Loi"] = "Không thể kết nối Web API. Hãy kiểm tra API đang chạy và địa chỉ/cổng kết nối.";
                return QuayLaiChiTiet();
            }
            catch (Exception)
            {
                TempData["Loi"] = "Đã xảy ra lỗi khi xử lý đặt phòng. Vui lòng thử lại.";
                return QuayLaiChiTiet();
            }
        }

        private static async Task<List<PhongViewModel>> GetRoomsAsync(string requestUri)
        {
            using var response = await _client.GetAsync(requestUri);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<PhongViewModel>>()
                   ?? new List<PhongViewModel>();
        }

        private static string FormatApiError(System.Net.HttpStatusCode statusCode, string body)
        {
            return string.IsNullOrWhiteSpace(body)
                ? $"API trả về {(int)statusCode} ({statusCode})."
                : $"API trả về {(int)statusCode}: {body}";
        }

        private static int? TryReadCustomerId(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            try
            {
                using var document = JsonDocument.Parse(json);
                return FindId(document.RootElement);
            }
            catch (JsonException)
            {
                return null;
            }
        }

        private static int? FindId(JsonElement element)
        {
            if (element.ValueKind == JsonValueKind.Number && element.TryGetInt32(out int rootId))
                return rootId;

            if (element.ValueKind == JsonValueKind.String && int.TryParse(element.GetString(), out rootId))
                return rootId;

            if (element.ValueKind == JsonValueKind.Object)
            {
                string[] idNames = { "KhachHangId", "khachHangId", "CustomerId", "customerId", "Id", "id" };
                foreach (var name in idNames)
                {
                    if (element.TryGetProperty(name, out var value))
                    {
                        if (value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out int id))
                            return id;
                        if (value.ValueKind == JsonValueKind.String && int.TryParse(value.GetString(), out id))
                            return id;
                    }
                }

                foreach (var property in element.EnumerateObject())
                {
                    var nestedId = FindId(property.Value);
                    if (nestedId.HasValue)
                        return nestedId;
                }
            }
            else if (element.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in element.EnumerateArray())
                {
                    var nestedId = FindId(item);
                    if (nestedId.HasValue)
                        return nestedId;
                }
            }

            return null;
        }
    }
}
