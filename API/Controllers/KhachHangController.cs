using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuanLyKhachSanAPI.Models;
using QuanLyKhachSanAPI.Security;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace QuanLyKhachSanAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class KhachHangController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly JwtSigningSettings _jwt;

    public KhachHangController(AppDbContext context, JwtSigningSettings jwt)
    {
        _context = context;
        _jwt = jwt;
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

        bool trungSdt = await _context.KhachHangs.AnyAsync(k => k.SoDienThoai == soDienThoai);
        if (trungSdt)
            return BadRequest("Số điện thoại này đã được đăng ký.");

        if (!string.IsNullOrWhiteSpace(request.CCCD))
        {
            bool trungCccd = await _context.KhachHangs.AnyAsync(k => k.Cccd == cccd);
            if (trungCccd)
                return BadRequest("Số CCCD/CMND này đã tồn tại trong hệ thống.");
        }

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
        catch (Exception ex)
        {
            // Trả về lỗi chi tiết từ SQL Server / EF Core để biết chính xác cột nào bị vướng
            return BadRequest("Lỗi lưu DB: " + (ex.InnerException?.Message ?? ex.Message));
        }
    }

    [HttpPost("dang-nhap")]
    public async Task<IActionResult> DangNhap([FromBody] KhachHangDangNhapDto request)
    {
        if (string.IsNullOrWhiteSpace(request.TaiKhoan) || string.IsNullOrWhiteSpace(request.MatKhau))
            return BadRequest("Vui lòng nhập tài khoản và mật khẩu.");

        var taiKhoanInput = request.TaiKhoan.Trim();
        var khachHang = await _context.KhachHangs
            .FirstOrDefaultAsync(k => k.SoDienThoai == taiKhoanInput || k.Email == taiKhoanInput);

        if (khachHang == null)
            return NotFound("Không tìm thấy tài khoản.");
        if (!PasswordSecurity.Verify(request.MatKhau, khachHang.MatKhau, out bool needsUpgrade))
            return Unauthorized("Tài khoản hoặc mật khẩu không chính xác.");
        if (needsUpgrade)
        {
            khachHang.MatKhau = PasswordSecurity.Hash(request.MatKhau);
            await _context.SaveChangesAsync();
        }

        // Tạo JWT Token trả về cho Web Client (khớp với TaiKhoanController bên Web)
        var securityKey = new SymmetricSecurityKey(_jwt.Key);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, khachHang.KhachHangId.ToString()),
            new Claim(ClaimTypes.NameIdentifier, khachHang.KhachHangId.ToString()),
            new Claim(ClaimTypes.Name, khachHang.HoTen ?? "Khách hàng"),
            new Claim(ClaimTypes.MobilePhone, khachHang.SoDienThoai ?? ""),
            new Claim(ClaimTypes.Role, "KhachHang"),
            new Claim("VaiTro", "KhachHang"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.Now.AddHours(4),
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new
        {
            ThongBao = "Đăng nhập thành công!",
            Token = tokenString,
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
                k.DiaChi,
                LoaiKhach = !string.IsNullOrEmpty(k.MatKhau) ? "Thành viên (Web)" : "Vãng lai (Tại quầy)"
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
            .Select(k => new
            {
                k.KhachHangId,
                k.HoTen,
                k.Cccd,
                k.SoDienThoai,
                k.Email,
                k.DiaChi,
                LoaiKhach = !string.IsNullOrEmpty(k.MatKhau) ? "Thành viên (Web)" : "Vãng lai (Tại quầy)"
            })
            .ToListAsync();

        return Ok(result);
    }

    // POST: api/KhachHang
    [HttpPost]
    public async Task<IActionResult> Create(KhachHang khachHang)
    {
        if (string.IsNullOrWhiteSpace(khachHang.HoTen))
            return BadRequest("Họ tên không được để trống.");

        // Chuẩn hóa chuỗi rỗng thành null để không vi phạm ràng buộc UNIQUE của SQL Server
        khachHang.HoTen = khachHang.HoTen.Trim();
        khachHang.Cccd = string.IsNullOrWhiteSpace(khachHang.Cccd) ? null : khachHang.Cccd.Trim();
        khachHang.SoDienThoai = string.IsNullOrWhiteSpace(khachHang.SoDienThoai) ? null : khachHang.SoDienThoai.Trim();
        khachHang.Email = string.IsNullOrWhiteSpace(khachHang.Email) ? null : khachHang.Email.Trim();

        var khachCu = await _context.KhachHangs
            .FirstOrDefaultAsync(k =>
                (khachHang.Cccd != null && k.Cccd == khachHang.Cccd) ||
                (khachHang.SoDienThoai != null && k.SoDienThoai == khachHang.SoDienThoai));

        if (khachCu != null)
        {
            return Conflict("Số điện thoại hoặc CCCD đã thuộc khách hàng khác.");
        }

        khachHang.KhachHangId = 0;

        _context.KhachHangs.Add(khachHang);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = khachHang.KhachHangId },
            new
            {
                khachHang.KhachHangId,
                khachHang.HoTen,
                khachHang.Cccd,
                khachHang.SoDienThoai,
                khachHang.Email,
                khachHang.DiaChi
            });
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
        return Ok(new
        {
            khachHang.KhachHangId,
            khachHang.HoTen,
            khachHang.Cccd,
            khachHang.SoDienThoai,
            khachHang.Email,
            khachHang.DiaChi
        });
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