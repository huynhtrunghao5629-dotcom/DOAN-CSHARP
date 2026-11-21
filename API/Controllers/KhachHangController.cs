using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhachSanAPI.Models;
using QuanLyKhachSanAPI.Security;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

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

    private static bool EmailHopLe(string email)
    {
        return Regex.IsMatch(
            email,
            @"^[^@\s]+@[^@\s]+\.[^@\s]{2,}$",
            RegexOptions.CultureInvariant);
    }

    [HttpPost("dang-ky")]
    public async Task<IActionResult> DangKy([FromBody] KhachHangDangKyDto request)
    {
        if (string.IsNullOrWhiteSpace(request.HoTen) ||
            string.IsNullOrWhiteSpace(request.SoDienThoai) ||
            string.IsNullOrWhiteSpace(request.MatKhau))
            return BadRequest("Họ tên, số điện thoại và mật khẩu không được để trống.");

        var hoTen = request.HoTen.Trim();
        var soDienThoai = request.SoDienThoai.Trim();
        var cccd = string.IsNullOrWhiteSpace(request.CCCD) ? null : request.CCCD.Trim();
        var email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();

        if (hoTen.Length > 100 || soDienThoai.Length > 15 || (cccd?.Length ?? 0) > 20 ||
            (email?.Length ?? 0) > 100)
            return BadRequest("Thông tin đăng ký vượt quá độ dài cho phép.");

        if (email != null && !EmailHopLe(email))
            return BadRequest("Email phải có tên miền đầy đủ, ví dụ: example@gmail.com.");

        if (request.MatKhau.Length < 6)
            return BadRequest("Mật khẩu phải có ít nhất 6 ký tự.");

        // 1. Kiểm tra trùng Số điện thoại
        bool trungSdt = await _context.KhachHangs.AnyAsync(k => k.SoDienThoai == soDienThoai);
        if (trungSdt)
            return BadRequest("Số điện thoại này đã được đăng ký.");

        // 2. Kiểm tra trùng CCCD (Nguyên nhân chính gây ra lỗi UNIQUE KEY của bạn)
        if (!string.IsNullOrWhiteSpace(request.CCCD))
        {
            bool trungCccd = await _context.KhachHangs.AnyAsync(k => k.Cccd == cccd);
            if (trungCccd)
                return BadRequest("Số CCCD/CMND này đã tồn tại trong hệ thống.");
        }

        // 3. Kiểm tra trùng Email (nếu người dùng có nhập)
        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            bool trungEmail = await _context.KhachHangs.AnyAsync(k => k.Email == email);
            if (trungEmail)
                return BadRequest("Email này đã được sử dụng.");
        }

        var khachHang = new QuanLyKhachSanAPI.Models.KhachHang
        {
            HoTen = hoTen,
            SoDienThoai = soDienThoai,
            Cccd = cccd,
            Email = email,
            MatKhau = PasswordSecurity.Hash(request.MatKhau)
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
        if (!PasswordSecurity.Verify(request.MatKhau, khachHang.MatKhau, out bool needsUpgrade))
            return Unauthorized("Tài khoản hoặc mật khẩu không chính xác.");
        if (needsUpgrade)
        {
            khachHang.MatKhau = PasswordSecurity.Hash(request.MatKhau);
            await _context.SaveChangesAsync();
        }

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
        var khachHangs = await _context.KhachHangs.AsNoTracking()
            .Select(k => new
            {
                k.KhachHangId,
                k.HoTen,
                k.Cccd,
                k.SoDienThoai,
                k.Email,
                k.DiaChi
            }).ToListAsync();
        return Ok(khachHangs);
    }

    // GET: api/KhachHang/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var kh = await _context.KhachHangs.AsNoTracking()
            .Where(k => k.KhachHangId == id)
            .Select(k => new { k.KhachHangId, k.HoTen, k.Cccd, k.SoDienThoai, k.Email, k.DiaChi })
            .FirstOrDefaultAsync();
        if (kh == null)
            return NotFound("Không tìm thấy khách hàng.");

        return Ok(kh);
    }

    [HttpGet("tim-kiem")]
    public async Task<IActionResult> Search([FromQuery] string tuKhoa)
    {
        if (string.IsNullOrWhiteSpace(tuKhoa))
            return Ok(await _context.KhachHangs.AsNoTracking()
                .Select(k => new { k.KhachHangId, k.HoTen, k.Cccd, k.SoDienThoai, k.Email, k.DiaChi })
                .ToListAsync());

        var result = await _context.KhachHangs
            .Where(k => k.HoTen.Contains(tuKhoa) 
                     || (k.SoDienThoai != null && k.SoDienThoai.Contains(tuKhoa)) 
                     || (k.Cccd != null && k.Cccd.Contains(tuKhoa)))
            .Select(k => new { k.KhachHangId, k.HoTen, k.Cccd, k.SoDienThoai, k.Email, k.DiaChi })
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
            // Never silently overwrite an existing customer's profile by matching phone/CCCD.
            return Conflict("Số điện thoại hoặc CCCD đã thuộc khách hàng khác.");
        }

        khachHang.KhachHangId = 0;

        if (!string.IsNullOrWhiteSpace(khachHang.Cccd)) khachHang.Cccd = khachHang.Cccd.Trim();
        if (!string.IsNullOrWhiteSpace(khachHang.SoDienThoai)) khachHang.SoDienThoai = khachHang.SoDienThoai.Trim();
        if (!string.IsNullOrWhiteSpace(khachHang.HoTen)) khachHang.HoTen = khachHang.HoTen.Trim();

        _context.KhachHangs.Add(khachHang);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = khachHang.KhachHangId },
            new { khachHang.KhachHangId, khachHang.HoTen, khachHang.Cccd,
                  khachHang.SoDienThoai, khachHang.Email, khachHang.DiaChi });
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

        var emailCapNhat = string.IsNullOrWhiteSpace(model.Email) ? null : model.Email.Trim();
        if (emailCapNhat != null && !EmailHopLe(emailCapNhat))
            return BadRequest("Email phải có tên miền đầy đủ, ví dụ: example@gmail.com.");

        if (emailCapNhat != null)
        {
            bool trungEmail = await _context.KhachHangs
                .AnyAsync(k => k.KhachHangId != id && k.Email == emailCapNhat);
            if (trungEmail)
                return BadRequest("Email này đã được sử dụng bởi tài khoản khác.");
        }

        khachHang.HoTen = model.HoTen.Trim();
        khachHang.Cccd = string.IsNullOrWhiteSpace(model.Cccd) ? null : model.Cccd.Trim();
        khachHang.SoDienThoai = string.IsNullOrWhiteSpace(model.SoDienThoai) ? null : model.SoDienThoai.Trim();
        khachHang.Email = emailCapNhat;
        khachHang.DiaChi = model.DiaChi;


        await _context.SaveChangesAsync();
        return Ok(new { khachHang.KhachHangId, khachHang.HoTen, khachHang.Cccd,
            khachHang.SoDienThoai, khachHang.Email, khachHang.DiaChi });
    }
    [HttpPut("cap-nhat-ho-so")]
    public async Task<IActionResult> CapNhatHoSo([FromBody] CapNhatHoSoDto request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.SoDienThoai) ||
            string.IsNullOrWhiteSpace(request.HoTen))
            return BadRequest("Thiếu số điện thoại hoặc họ tên.");

        var soDienThoai = request.SoDienThoai.Trim();
        var hoTen = request.HoTen.Trim();
        var cccd = string.IsNullOrWhiteSpace(request.CCCD) ? null : request.CCCD.Trim();
        var email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();

        if (hoTen.Length > 100 || (cccd?.Length ?? 0) > 20 || (email?.Length ?? 0) > 100)
            return BadRequest("Thông tin hồ sơ vượt quá độ dài cho phép.");

        if (email != null && !EmailHopLe(email))
            return BadRequest("Email phải có tên miền đầy đủ, ví dụ: example@gmail.com.");

        if (!string.IsNullOrWhiteSpace(request.MatKhauMoi) && request.MatKhauMoi.Length < 6)
            return BadRequest("Mật khẩu mới phải có ít nhất 6 ký tự.");

        var khachHang = await _context.KhachHangs
            .FirstOrDefaultAsync(k => k.SoDienThoai == soDienThoai);

        if (khachHang == null)
            return NotFound("Không tìm thấy tài khoản.");

        if (cccd != null && !string.Equals(cccd, khachHang.Cccd, StringComparison.Ordinal))
        {
            bool trungCccd = await _context.KhachHangs.AnyAsync(k =>
                k.KhachHangId != khachHang.KhachHangId && k.Cccd == cccd);
            if (trungCccd)
                return BadRequest("CCCD này đã được đăng ký cho tài khoản khác.");
        }

        if (email != null && !string.Equals(email, khachHang.Email, StringComparison.OrdinalIgnoreCase))
        {
            bool trungEmail = await _context.KhachHangs.AnyAsync(k =>
                k.KhachHangId != khachHang.KhachHangId && k.Email == email);
            if (trungEmail)
                return BadRequest("Email này đã được sử dụng bởi tài khoản khác.");
        }

        khachHang.HoTen = hoTen;
        khachHang.Cccd = cccd;
        khachHang.Email = email;

        if (!string.IsNullOrWhiteSpace(request.MatKhauMoi))
            khachHang.MatKhau = PasswordSecurity.Hash(request.MatKhauMoi);

        await _context.SaveChangesAsync();
        return Ok(new
        {
            Message = "Cập nhật thành công",
            khachHang.KhachHangId,
            khachHang.HoTen,
            khachHang.SoDienThoai,
            khachHang.Cccd,
            khachHang.Email
        });
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