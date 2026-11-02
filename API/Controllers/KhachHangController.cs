using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhachSanAPI.Models;

namespace QuanLyKhachSanAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class KhachHangController : ControllerBase
{
    private readonly AppDbContext _context;

    public KhachHangController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("dang-ky")]
    public async Task<IActionResult> DangKy([FromBody] KhachHangDangKyDto request)
    {
        if (string.IsNullOrWhiteSpace(request.SoDienThoai) || string.IsNullOrWhiteSpace(request.MatKhau))
            return BadRequest("Số điện thoại và mật khẩu không được để trống.");

        // 1. Kiểm tra trùng Số điện thoại
        bool trungSdt = await _context.KhachHangs.AnyAsync(k => k.SoDienThoai == request.SoDienThoai.Trim());
        if (trungSdt)
            return BadRequest("Số điện thoại này đã được đăng ký.");

        // 2. Kiểm tra trùng CCCD (Nguyên nhân chính gây ra lỗi UNIQUE KEY của bạn)
        if (!string.IsNullOrWhiteSpace(request.CCCD))
        {
            bool trungCccd = await _context.KhachHangs.AnyAsync(k => k.Cccd == request.CCCD.Trim());
            if (trungCccd)
                return BadRequest("Số CCCD/CMND này đã tồn tại trong hệ thống.");
        }

        // 3. Kiểm tra trùng Email (nếu người dùng có nhập)
        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            bool trungEmail = await _context.KhachHangs.AnyAsync(k => k.Email == request.Email.Trim());
            if (trungEmail)
                return BadRequest("Email này đã được sử dụng.");
        }

        var khachHang = new QuanLyKhachSanAPI.Models.KhachHang
        {
            HoTen = request.HoTen.Trim(),
            SoDienThoai = request.SoDienThoai.Trim(),
            Cccd = request.CCCD.Trim(),
            Email = request.Email?.Trim(),
            MatKhau = request.MatKhau
        };

        try
        {
            _context.KhachHangs.Add(khachHang);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Đăng ký thành công!" });
        }
        catch (Exception)
        {
            // Bắt lỗi an toàn nếu cấu trúc DB còn ràng buộc UNIQUE nào khác
            return BadRequest("Không thể đăng ký: Dữ liệu (SĐT hoặc CCCD) bị trùng lặp với hệ thống.");
        }
    }

    [HttpPost("dang-nhap")]
    public async Task<IActionResult> DangNhap([FromBody] KhachHangDangNhapDto request)
    {
        if (string.IsNullOrWhiteSpace(request.TaiKhoan) || string.IsNullOrWhiteSpace(request.MatKhau))
            return BadRequest("Vui lòng nhập tài khoản và mật khẩu.");

        var khachHang = await _context.KhachHangs
            .FirstOrDefaultAsync(k => k.SoDienThoai == request.TaiKhoan.Trim() || k.Email == request.TaiKhoan.Trim());

        if (khachHang == null)
            return NotFound("Không tìm thấy tài khoản.");
        if (khachHang.MatKhau != request.MatKhau) 
            return BadRequest("Sai mật khẩu.");

        return Ok(new
        {
            KhachHangId = khachHang.KhachHangId,
            HoTen = khachHang.HoTen,
            SoDienThoai = khachHang.SoDienThoai
        });
    }
    // GET: api/KhachHang
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var khachHangs = await _context.KhachHangs.ToListAsync();
        return Ok(khachHangs);
    }

    // GET: api/KhachHang/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var kh = await _context.KhachHangs.FindAsync(id);
        if (kh == null)
            return NotFound("Không tìm thấy khách hàng.");

        return Ok(kh);
    }

    [HttpGet("tim-kiem")]
    public async Task<IActionResult> Search([FromQuery] string tuKhoa)
    {
        if (string.IsNullOrWhiteSpace(tuKhoa))
            return Ok(await _context.KhachHangs.ToListAsync());

        var result = await _context.KhachHangs
            .Where(k => k.HoTen.Contains(tuKhoa) 
                     || (k.SoDienThoai != null && k.SoDienThoai.Contains(tuKhoa)) 
                     || (k.Cccd != null && k.Cccd.Contains(tuKhoa)))
            .ToListAsync();

        return Ok(result);
    }

    // POST: api/KhachHang
    [HttpPost]
    public async Task<IActionResult> Create(KhachHang khachHang)
    {
        if (string.IsNullOrWhiteSpace(khachHang.HoTen))
            return BadRequest("Họ tên không được để trống.");

        var khachCu = await _context.KhachHangs
            .FirstOrDefaultAsync(k =>
                (!string.IsNullOrWhiteSpace(khachHang.Cccd) && k.Cccd == khachHang.Cccd.Trim()) ||
                (!string.IsNullOrWhiteSpace(khachHang.SoDienThoai) && k.SoDienThoai == khachHang.SoDienThoai.Trim()));

        if (khachCu != null)
        {
            khachCu.HoTen = khachHang.HoTen.Trim();

            if (!string.IsNullOrWhiteSpace(khachHang.SoDienThoai)) khachCu.SoDienThoai = khachHang.SoDienThoai.Trim();
            if (!string.IsNullOrWhiteSpace(khachHang.Cccd)) khachCu.Cccd = khachHang.Cccd.Trim();
            if (!string.IsNullOrWhiteSpace(khachHang.Email)) khachCu.Email = khachHang.Email.Trim();

            await _context.SaveChangesAsync();

            return Ok(khachCu);
        }

        khachHang.KhachHangId = 0;

        if (!string.IsNullOrWhiteSpace(khachHang.Cccd)) khachHang.Cccd = khachHang.Cccd.Trim();
        if (!string.IsNullOrWhiteSpace(khachHang.SoDienThoai)) khachHang.SoDienThoai = khachHang.SoDienThoai.Trim();
        if (!string.IsNullOrWhiteSpace(khachHang.HoTen)) khachHang.HoTen = khachHang.HoTen.Trim();

        _context.KhachHangs.Add(khachHang);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = khachHang.KhachHangId }, khachHang);
    }

    // PUT: api/KhachHang/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, KhachHang model)
    {
        var khachHang = await _context.KhachHangs.FindAsync(id);
        if (khachHang == null)
            return NotFound("Không tìm thấy khách hàng.");

        if (string.IsNullOrWhiteSpace(model.HoTen))
            return BadRequest("Họ tên không được để trống.");

        if (!string.IsNullOrWhiteSpace(model.Cccd))
        {
            bool trungCccd = await _context.KhachHangs
                .AnyAsync(k => k.Cccd == model.Cccd && k.KhachHangId != id);
            if (trungCccd)
                return BadRequest("Số CCCD đã tồn tại ở khách hàng khác.");
        }

        khachHang.HoTen = model.HoTen;
        khachHang.Cccd = model.Cccd;
        khachHang.SoDienThoai = model.SoDienThoai;
        khachHang.Email = model.Email;
        khachHang.DiaChi = model.DiaChi;


        await _context.SaveChangesAsync();
        return Ok(khachHang);
    }
    [HttpPut("cap-nhat-ho-so")]
    public async Task<IActionResult> CapNhatHoSo([FromBody] CapNhatHoSoDto request)
    {
        var khachHang = await _context.KhachHangs.FirstOrDefaultAsync(k => k.SoDienThoai == request.SoDienThoai);
        if (khachHang == null) return NotFound("Không tìm thấy tài khoản.");

        // Kiểm tra CCCD có bị trùng với người khác không
        if (!string.IsNullOrWhiteSpace(request.CCCD) && request.CCCD != khachHang.Cccd)
        {
            bool trungCccd = await _context.KhachHangs.AnyAsync(k => k.Cccd == request.CCCD.Trim());
            if (trungCccd) return BadRequest("CCCD này đã được đăng ký cho tài khoản khác.");
        }

        // Cập nhật thông tin
        khachHang.HoTen = request.HoTen.Trim();
        if (!string.IsNullOrWhiteSpace(request.CCCD)) khachHang.Cccd = request.CCCD.Trim();
        khachHang.Email = request.Email?.Trim();

        // Cập nhật mật khẩu nếu khách hàng có nhập mật khẩu mới
        if (!string.IsNullOrWhiteSpace(request.MatKhauMoi))
            khachHang.MatKhau = request.MatKhauMoi;

        await _context.SaveChangesAsync();
        return Ok(new { Message = "Cập nhật thành công" });
    }

    public class CapNhatHoSoDto
    {
        public string SoDienThoai { get; set; } = string.Empty;
        public string HoTen { get; set; } = string.Empty;
        public string? CCCD { get; set; }
        public string? Email { get; set; }
        public string? MatKhauMoi { get; set; }
    }
    public class KhachHangDangKyDto
    {
        public string HoTen { get; set; } = string.Empty;
        public string SoDienThoai { get; set; } = string.Empty;
        public string CCCD { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string MatKhau { get; set; } = string.Empty;
    }

    public class KhachHangDangNhapDto
    {
        public string TaiKhoan { get; set; } = string.Empty;
        public string MatKhau { get; set; } = string.Empty;
    }
}