using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhachSanAPI.Models;

namespace QuanLyKhachSanAPI.Controllers
{
    public class PhongDtoRequest
    {
        public string SoPhong { get; set; } = string.Empty;
        public int LoaiPhongId { get; set; }
        public string TrangThai { get; set; } = string.Empty;
    }

    // Chuẩn hóa cấu trúc JSON cho tất cả endpoint GET để Web MVC/WinForms
    // không phải xử lý các tên trường khác nhau (LoaiPhong/TenLoaiPhong/TenLoai).
    public class PhongResponseDto
    {
        public int PhongId { get; set; }
        public string SoPhong { get; set; } = string.Empty;
        public string TrangThai { get; set; } = string.Empty;
        public int LoaiPhongId { get; set; }
        public string TenLoaiPhong { get; set; } = string.Empty;
        public decimal GiaPhong { get; set; }
        public string? HinhAnh { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class PhongController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PhongController(AppDbContext context)
        {
            _context = context;
        }

        private IQueryable<PhongResponseDto> QueryPhong()
        {
            return _context.Phongs
                .AsNoTracking()
                .Select(p => new PhongResponseDto
                {
                    PhongId = p.PhongId,
                    SoPhong = p.SoPhong,
                    TrangThai = p.TrangThai,
                    LoaiPhongId = p.LoaiPhongId,
                    TenLoaiPhong = p.LoaiPhong.TenLoai,
                    GiaPhong = p.LoaiPhong.GiaPhong,
                    HinhAnh = p.HinhAnh
                });
        }

        // GET: api/Phong
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var rooms = await QueryPhong().OrderBy(p => p.SoPhong).ToListAsync();
            return Ok(rooms);
        }

        // GET: api/Phong/phan-trang?page=1&pageSize=10
        [HttpGet("phan-trang")]
        public async Task<IActionResult> GetPhongsPhanTrang([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = QueryPhong().OrderBy(p => p.SoPhong);
            var totalRecords = await query.CountAsync();
            var rooms = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return Ok(new
            {
                TongSoPhong = totalRecords,
                TrangHienTai = page,
                KichThuocTrang = pageSize,
                TongSoTrang = (int)Math.Ceiling(totalRecords / (double)pageSize),
                DuLieu = rooms
            });
        }

        // GET: api/Phong/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var room = await QueryPhong().FirstOrDefaultAsync(p => p.PhongId == id);
            if (room == null)
                return NotFound("Không tìm thấy phòng.");

            return Ok(room);
        }

        // GET: api/Phong/loai/1
        [HttpGet("loai/{loaiPhongId:int}")]
        public async Task<IActionResult> GetByLoai(int loaiPhongId)
        {
            var rooms = await QueryPhong()
                .Where(p => p.LoaiPhongId == loaiPhongId)
                .OrderBy(p => p.SoPhong)
                .ToListAsync();

            return Ok(rooms);
        }

        // GET: api/Phong/trong
        [HttpGet("trong")]
        public async Task<IActionResult> GetPhongTrong()
        {
            var rooms = await QueryPhong()
                .Where(p => p.TrangThai.Trim().ToLower() == "trong" ||
                            p.TrangThai.Trim().ToLower() == "trống" ||
                            p.TrangThai.Trim().ToLower() == "available")
                .OrderBy(p => p.SoPhong)
                .ToListAsync();

            return Ok(rooms);
        }

        [HttpGet("tim-kiem")]
        public async Task<IActionResult> TimKiemPhong(
            [FromQuery] DateTime? ngayNhan,
            [FromQuery] DateTime? ngayTra,
            [FromQuery] decimal? giaTu,
            [FromQuery] decimal? giaDen)
        {
            if (ngayNhan.HasValue != ngayTra.HasValue)
                return BadRequest("Cần cung cấp cả ngày nhận và ngày trả phòng.");

            if (ngayNhan.HasValue && ngayTra.HasValue)
            {
                // Fix lỗi DateOnly: Dùng DateTime.Today để so sánh với phần Date của ngayNhan
                if (ngayNhan.Value.Date < DateTime.Today)
                    return BadRequest("Ngày nhận phòng không được ở trong quá khứ.");

                if (ngayTra.Value <= ngayNhan.Value)
                    return BadRequest("Ngày trả phòng phải sau ngày nhận phòng.");
            }

            if (giaTu.HasValue && giaDen.HasValue && giaTu.Value > giaDen.Value)
                return BadRequest("Giá tối thiểu không được lớn hơn giá tối đa.");

            var roomQuery = _context.Phongs
                .AsNoTracking()
                .Where(p => p.TrangThai == null ||
                            (p.TrangThai.Trim().ToLower() != "bao tri" &&
                             p.TrangThai.Trim().ToLower() != "bảo trì" &&
                             p.TrangThai.Trim().ToLower() != "maintenance" &&
                             p.TrangThai.Trim().ToLower() != "da dat" &&
                             p.TrangThai.Trim().ToLower() != "đã đặt" &&
                             p.TrangThai.Trim().ToLower() != "booked"));

            if (giaTu.HasValue)
                roomQuery = roomQuery.Where(p => p.LoaiPhong.GiaPhong >= giaTu.Value);

            if (giaDen.HasValue)
                roomQuery = roomQuery.Where(p => p.LoaiPhong.GiaPhong <= giaDen.Value);

            if (ngayNhan.HasValue && ngayTra.HasValue)
            {
                var ngayVao = ngayNhan.Value;
                var ngayRa = ngayTra.Value;

                var phongBiDat = await _context.ChiTietDatPhongs
                    .AsNoTracking()
                    .Where(ct =>
                        ct.DatPhong != null &&
                        (ct.DatPhong.TrangThai == null ||
                         (ct.DatPhong.TrangThai.ToLower() != "dahuy" &&
                          ct.DatPhong.TrangThai.ToLower() != "đã hủy" &&
                          ct.DatPhong.TrangThai.ToLower() != "da huy" &&
                          ct.DatPhong.TrangThai.ToLower() != "datra" &&
                          ct.DatPhong.TrangThai.ToLower() != "đã trả" &&
                          ct.DatPhong.TrangThai.ToLower() != "da tra" &&
                          ct.DatPhong.TrangThai.ToLower() != "cancelled" &&
                          ct.DatPhong.TrangThai.ToLower() != "checkedout")) &&
                        ct.DatPhong.NgayNhan < ngayRa &&
                        ct.DatPhong.NgayTra > ngayVao)
                    .Select(ct => ct.PhongId)
                    .Distinct()
                    .ToListAsync();

                // Dùng .Any() thay cho .Count > 0 để tránh lỗi method group
                if (phongBiDat.Any())
                    roomQuery = roomQuery.Where(p => !phongBiDat.Contains(p.PhongId));
            }

            var rooms = await roomQuery
                .Select(p => new PhongResponseDto
                {
                    PhongId = p.PhongId,
                    SoPhong = p.SoPhong,
                    TrangThai = p.TrangThai,
                    LoaiPhongId = p.LoaiPhongId,
                    TenLoaiPhong = p.LoaiPhong.TenLoai,
                    GiaPhong = p.LoaiPhong.GiaPhong,
                    HinhAnh = p.HinhAnh
                })
                .OrderBy(p => p.SoPhong)
                .ToListAsync();

            return Ok(rooms);
        }

        // POST: api/Phong
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PhongDtoRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.SoPhong))
                return BadRequest("Số phòng không được để trống.");

            string soPhong = request.SoPhong.Trim();
            bool trungSoPhong = await _context.Phongs.AnyAsync(p => p.SoPhong == soPhong);
            if (trungSoPhong)
                return BadRequest("Số phòng đã tồn tại.");

            bool loaiPhongTonTai = await _context.LoaiPhongs.AnyAsync(lp => lp.LoaiPhongId == request.LoaiPhongId);
            if (!loaiPhongTonTai)
                return BadRequest("Loại phòng không tồn tại.");

            var room = new QuanLyKhachSanAPI.Models.Phong
            {
                SoPhong = soPhong,
                LoaiPhongId = request.LoaiPhongId,
                TrangThai = string.IsNullOrWhiteSpace(request.TrangThai) ? "Trong" : request.TrangThai.Trim()
            };

            _context.Phongs.Add(room);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = room.PhongId }, room);
        }

        // PUT: api/Phong/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] PhongDtoRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.SoPhong))
                return BadRequest("Số phòng không được để trống.");

            var room = await _context.Phongs.FindAsync(id);
            if (room == null)
                return NotFound("Không tìm thấy phòng.");

            string soPhong = request.SoPhong.Trim();
            bool trungSoPhong = await _context.Phongs.AnyAsync(p => p.SoPhong == soPhong && p.PhongId != id);
            if (trungSoPhong)
                return BadRequest("Số phòng đã tồn tại.");

            bool loaiPhongTonTai = await _context.LoaiPhongs.AnyAsync(lp => lp.LoaiPhongId == request.LoaiPhongId);
            if (!loaiPhongTonTai)
                return BadRequest("Loại phòng không tồn tại.");

            room.SoPhong = soPhong;
            room.LoaiPhongId = request.LoaiPhongId;
            room.TrangThai = string.IsNullOrWhiteSpace(request.TrangThai) ? "Trong" : request.TrangThai.Trim();

            await _context.SaveChangesAsync();
            return Ok(await QueryPhong().FirstAsync(p => p.PhongId == id));
        }

        // DELETE: api/Phong/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var room = await _context.Phongs.FindAsync(id);
            if (room == null)
                return NotFound("Không tìm thấy phòng.");

            bool dangDuocDat = await _context.ChiTietDatPhongs.AnyAsync(ct => ct.PhongId == id);
            if (dangDuocDat)
                return BadRequest("Phòng đã có lịch đặt, không thể xóa.");

            _context.Phongs.Remove(room);
            await _context.SaveChangesAsync();
            return Ok("Xóa phòng thành công.");
        }
    }
}
