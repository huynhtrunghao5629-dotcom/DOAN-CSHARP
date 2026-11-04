using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using QuanLyKhachSan.Web.Models;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Net;

namespace QuanLyKhachSan.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _client;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient("HotelApi");
        }

        [HttpGet]
        public async Task<IActionResult> DichVu()
        {
            ViewData["NavbarOverlay"] = false;
            try
            {
                using var response = await _client.GetAsync("api/DichVu");
                response.EnsureSuccessStatusCode();
                return View(await response.Content.ReadFromJsonAsync<List<DichVuKhachViewModel>>() ?? new());
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
            {
                ViewBag.ApiError = "Chưa thể tải dịch vụ từ hệ thống. Vui lòng thử lại sau.";
                return View(new List<DichVuKhachViewModel>());
            }
        }

        [HttpGet]
        public IActionResult HuongDan() => View();

        [HttpGet]
        public IActionResult Privacy() => View();

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
                ViewBag.ApiError = "Không thể tải dữ liệu phòng. Vui lòng kiểm tra kết nối API.";
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
    int? soKhach = null,
    string? tuKhoa = null,
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
            ViewBag.SoKhach = soKhach;
            ViewBag.TuKhoa = tuKhoa?.Trim();

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
                    rooms = new();
                }
                else if (coNgayNhan && coNgayTra)
                {
                    if (ngayNhan!.Value < DateOnly.FromDateTime(DateTime.Today) || ngayTra!.Value <= ngayNhan.Value)
                    {
                        ViewBag.FilterError = "Ngày nhận phòng phải từ hôm nay trở đi và ngày trả phòng phải sau ngày nhận phòng.";
                        rooms = new();
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

                // 3. Loại phòng được tải từ API, không mặc định ID cố định.
                // Thông tin sức chứa nằm ở LoaiPhong, không có trong PhongResponseDto.
                var roomTypes = await GetRoomTypesAsync();
                ViewBag.DanhSachLoaiPhong = roomTypes;
                if (roomTypes.Count > 0)
                {
                    var typeLookup = roomTypes.ToDictionary(t => t.LoaiPhongId);
                    foreach (var room in rooms)
                    {
                        if (typeLookup.TryGetValue(room.LoaiPhongId, out var roomType))
                        {
                            room.SoNguoiToiDa = roomType.SoNguoiToiDa > 0 ? roomType.SoNguoiToiDa : null;
                            room.MoTaLoaiPhong = roomType.MoTa;
                        }
                    }
                }

                if (soKhach.HasValue)
                {
                    if (soKhach.Value < 1 || soKhach.Value > 30)
                    {
                        ViewBag.FilterError = "Số khách phải từ 1 đến 30 người.";
                        rooms.Clear();
                    }
                    else if (roomTypes.Count == 0)
                    {
                        ViewBag.FilterError = "Chưa tải được thông tin sức chứa từ API. Vui lòng thử lại sau.";
                        rooms.Clear();
                    }
                    else
                    {
                        if (rooms.Any(p => !p.SoNguoiToiDa.HasValue))
                            ViewBag.FilterNotice = "Một số phòng thiếu thông tin sức chứa nên không xuất hiện trong kết quả lọc.";
                        rooms = rooms.Where(p => p.SoNguoiToiDa >= soKhach.Value).ToList();
                    }
                }

                // Tìm không phân biệt dấu: "doi" khớp "đôi", "phong" khớp "phòng".
                // So sánh cả tên loại từ API và tên đã Việt hóa để không bỏ sót kết quả.
                if (!string.IsNullOrWhiteSpace(tuKhoa))
                    rooms = rooms.Where(p =>
                        VanBanTiengViet.ChuaTuKhoa(p.SoPhong, tuKhoa) ||
                        VanBanTiengViet.ChuaTuKhoa(p.TenLoaiPhong, tuKhoa) ||
                        VanBanTiengViet.ChuaTuKhoa(HienThiPhong.TenLoai(p.TenLoaiPhong), tuKhoa)).ToList();

                if ((giaTu.HasValue && giaTu < 0) || (giaDen.HasValue && giaDen < 0))
                {
                    ViewBag.FilterError = "Khoảng giá không được là số âm.";
                    rooms.Clear();
                }
                else if (giaTu.HasValue && giaDen.HasValue && giaTu.Value > giaDen.Value)
                {
                    ViewBag.FilterError = "Giá tối thiểu không được lớn hơn giá tối đa.";
                    rooms.Clear();
                }
                // 4. Lọc theo checkbox "Chỉ hiển thị phòng trống"
                if (chiPhongTrong && !(coNgayNhan && coNgayTra && string.IsNullOrEmpty(ViewBag.FilterError as string)))
                {
                    rooms = rooms.Where(p => HienThiPhong.DangTrong(p.TrangThai)).ToList();
                }

                // 5. Lọc theo Hạng phòng
                if (loaiPhong.HasValue && loaiPhong.Value > 0)
                {
                    rooms = rooms.Where(p => p.LoaiPhongId == loaiPhong.Value).ToList();
                }

                // 6. Lọc theo Khoảng giá (Từ - Đến)
                if (giaTu.HasValue && giaTu.Value >= 0)
                {
                    rooms = rooms.Where(p => p.GiaPhong >= giaTu.Value).ToList();
                }
                if (giaDen.HasValue && giaDen.Value > 0)
                {
                    rooms = rooms.Where(p => p.GiaPhong <= giaDen.Value).ToList();
                }

                // 7. Sắp xếp giá Tăng/Giảm
                if (!string.IsNullOrEmpty(sapXepGia))
                {
                    if (sapXepGia == "tang")
                        rooms = rooms.OrderBy(p => p.GiaPhong).ToList();
                    else if (sapXepGia == "giam")
                        rooms = rooms.OrderByDescending(p => p.GiaPhong).ToList();
                }

                // 8. Logic Phân trang (Cắt dữ liệu mang ra View)
                int soPhongMoiTrang = 6;
                int tongSoPhong = rooms.Count;
                int tongSoTrang = (int)Math.Ceiling((double)tongSoPhong / soPhongMoiTrang);

                if (trang < 1) trang = 1;
                if (tongSoTrang > 0 && trang > tongSoTrang) trang = tongSoTrang;

                var phongTrenTrang = rooms
                    .Skip((trang - 1) * soPhongMoiTrang)
                    .Take(soPhongMoiTrang)
                    .ToList();

                ViewBag.TongSoPhong = tongSoPhong;
                ViewBag.TrangHienTai = trang;
                ViewBag.TongSoTrang = tongSoTrang;

                return View(phongTrenTrang);
            }
            catch (Exception)
            {
                ViewBag.ApiError = "Không thể tải danh sách phòng. Vui lòng kiểm tra kết nối Web API.";
                ViewBag.DanhSachLoaiPhong = new List<LoaiPhongViewModel>();
                ViewBag.TongSoPhong = 0;
                return View(new List<PhongViewModel>());
            }
        }

        public async Task<IActionResult> ChiTietPhong(int id, DateOnly? ngayNhan = null, DateOnly? ngayTra = null, int? soKhach = null)
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

                var types = await GetRoomTypesAsync();
                var type = types.FirstOrDefault(t => t.LoaiPhongId == room.LoaiPhongId);
                room.SoNguoiToiDa = type?.SoNguoiToiDa > 0 ? type.SoNguoiToiDa : null;
                room.MoTaLoaiPhong = type?.MoTa;
                ViewBag.NgayNhan = ngayNhan;
                ViewBag.NgayTra = ngayTra;
                ViewBag.SoKhach = soKhach;
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

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DatPhong(
    int PhongId,
    DateTime? NgayNhan,
    DateTime? NgayTra,
    int? SoKhach)
        {
            IActionResult QuayLaiChiTiet() => RedirectToAction(nameof(ChiTietPhong), new
            {
                id = PhongId,
                ngayNhan = NgayNhan.HasValue && NgayNhan.Value.Date >= DateTime.Today ? NgayNhan.Value.ToString("yyyy-MM-dd") : null,
                ngayTra = NgayTra.HasValue && NgayTra.Value.Date > DateTime.Today ? NgayTra.Value.ToString("yyyy-MM-dd") : null,
                soKhach = SoKhach
            })!;

            if (PhongId <= 0)
            {
                TempData["Loi"] = "Mã phòng không hợp lệ.";
                return RedirectToAction(nameof(DanhSachPhong));
            }

            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var khachHangId) || khachHangId <= 0)
            {
                TempData["Loi"] = "Phiên đăng nhập không hợp lệ. Vui lòng đăng nhập lại.";
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            if (SoKhach is < 1 or > 30)
            {
                TempData["Loi"] = "Số khách lưu trú không hợp lệ.";
                return QuayLaiChiTiet();
            }

            if (!NgayNhan.HasValue || !NgayTra.HasValue ||
                    NgayNhan.Value.Date < DateTime.Today ||
                    NgayTra.Value <= NgayNhan.Value)
            {
                TempData["Loi"] = "Thời gian nhận phòng không hợp lệ hoặc thời gian trả phòng phải sau thời gian nhận phòng.";
                return QuayLaiChiTiet();
            }

            try
            {
                // === BƯỚC 1: KIỂM TRA TRẠNG THÁI HIỆN TẠI CỦA PHÒNG ===
                using var roomResponse = await _client.GetAsync($"api/Phong/{PhongId}");
                if (!roomResponse.IsSuccessStatusCode)
                {
                    TempData["Loi"] = "Không xác nhận được thông tin phòng. Vui lòng thử lại.";
                    return QuayLaiChiTiet();
                }
                if (roomResponse.IsSuccessStatusCode)
                {
                    var room = await roomResponse.Content.ReadFromJsonAsync<PhongViewModel>();
                    if (room != null)
                    {
                        var status = (room.TrangThai ?? string.Empty).Trim().ToLowerInvariant();
                        // Chỉ chặn trạng thái bảo trì; phòng có đơn tương lai vẫn có thể trống theo ngày.
                        // API phải kiểm tra trùng lịch lần cuối khi ghi đơn.
                        if (HienThiPhong.DangBaoTri(room.TrangThai))
                        {
                            TempData["Loi"] = "Phòng đang bảo trì nên chưa thể nhận đặt phòng.";
                            return QuayLaiChiTiet();
                        }
                        if (SoKhach.HasValue)
                        {
                            var types = await GetRoomTypesAsync();
                            var roomType = types.FirstOrDefault(t => t.LoaiPhongId == room.LoaiPhongId);
                            if (roomType?.SoNguoiToiDa > 0 && SoKhach.Value > roomType.SoNguoiToiDa)
                            {
                                TempData["Loi"] = $"Phòng này chỉ tiếp nhận tối đa {roomType.SoNguoiToiDa} người. Vui lòng giảm số khách hoặc chọn phòng khác.";
                                return QuayLaiChiTiet();
                            }
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

                // Dùng mã khách hàng đã xác thực trong cookie, không tạo hồ sơ khách khác
                // từ thông tin tùy ý nhập trên form.
                var bookingPayload = new
                {
                    KhachHangId = khachHangId,
                    NgayNhan = NgayNhan.Value, 
                    NgayTra = NgayTra.Value,   
                    DanhSachPhongId = new List<int> { PhongId }
                };

                using var bookingResponse = await _client.PostAsJsonAsync("api/DatPhong", bookingPayload);
                if (bookingResponse.IsSuccessStatusCode)
                {
                    // Read the booking ID returned from API, not from the customer object.
                    var bookingJson = await bookingResponse.Content.ReadAsStringAsync();
                    var bookingId = TryReadIdFromNamedProperty(bookingJson, "datPhongId");
                    if (bookingId.HasValue)
                        return RedirectToAction(nameof(XacNhanDatPhong), new { id = bookingId.Value });

                    TempData["ThongBao"] = "Đặt phòng thành công. Hãy xem lại đơn trong lịch sử đặt phòng.";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Loi"] = bookingResponse.StatusCode switch
                {
                    HttpStatusCode.BadRequest or HttpStatusCode.Conflict => "Thông tin đặt phòng không hợp lệ hoặc phòng vừa được người khác đặt. Vui lòng kiểm tra lại ngày lưu trú.",
                    HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden => "Không có quyền đặt phòng. Vui lòng đăng nhập lại.",
                    _ => "Không thể đặt phòng lúc này. Vui lòng thử lại hoặc liên hệ lễ tân."
                };
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

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> XacNhanDatPhong(int id)
        {
            if (id <= 0) return RedirectToAction(nameof(Index));
            var phone = User.FindFirstValue(ClaimTypes.MobilePhone);
            if (string.IsNullOrWhiteSpace(phone)) return Forbid();
            try
            {
                // Tránh cho truy cập bằng ID bất kỳ và hiển thị thông báo thành công giả.
                using var response = await _client.GetAsync($"api/DatPhong/tra-cuu?sdt={Uri.EscapeDataString(phone)}");
                response.EnsureSuccessStatusCode();
                var history = await response.Content.ReadFromJsonAsync<List<LichSuDatPhongViewModel>>() ?? new();
                var booking = history.FirstOrDefault(d => d.DatPhongId == id);
                if (booking == null) return NotFound("Không tìm thấy đơn đặt phòng của tài khoản.");
                ViewData["NavbarOverlay"] = false;
                ViewBag.MaDon = string.IsNullOrWhiteSpace(booking.MaDon) ? $"#DP-{id:D5}" : booking.MaDon;
                ViewBag.TrangThai = TrangThaiDatPhong.HienThi(booking.TrangThai);
                return View();
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
            {
                TempData["Loi"] = "Không thể kiểm tra xác nhận đơn lúc này. Vui lòng xem lịch sử đặt phòng.";
                return RedirectToAction("HoSoCaNhan", "TaiKhoan");
            }
        }

        private static int? TryReadIdFromNamedProperty(string json, string propertyName)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.ValueKind == JsonValueKind.Object &&
                    doc.RootElement.TryGetProperty(propertyName, out var value) &&
                    value.TryGetInt32(out var id) && id > 0) return id;
            }
            catch (JsonException) { }
            return null;
        }

        // Trả danh sách rỗng khi API loại phòng chưa sẵn sàng; không làm hỏng tìm phòng cơ bản.
        private async Task<List<LoaiPhongViewModel>> GetRoomTypesAsync()
        {
            try
            {
                using var response = await _client.GetAsync("api/LoaiPhong");
                if (!response.IsSuccessStatusCode) return new();
                return await response.Content.ReadFromJsonAsync<List<LoaiPhongViewModel>>() ?? new();
            }
            catch (HttpRequestException) { return new(); }
            catch (TaskCanceledException) { return new(); }
            catch (JsonException) { return new(); }
        }

        private async Task<List<PhongViewModel>> GetRoomsAsync(string requestUri)
        {
            using var response = await _client.GetAsync(requestUri);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<PhongViewModel>>()
                   ?? new List<PhongViewModel>();
        }


    }
}
