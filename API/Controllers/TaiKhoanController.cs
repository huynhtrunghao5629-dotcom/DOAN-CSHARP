using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens; // Thư viện JWT
using System.IdentityModel.Tokens.Jwt; // Thư viện JWT
using System.Security.Claims; // Thư viện JWT
using System.Text; // Thư viện JWT
using Microsoft.AspNetCore.Authorization;
using QuanLyKhachSanAPI.Security;
using QuanLyKhachSanAPI.Models;

namespace QuanLyKhachSanAPI.Controllers;

public class LoginRequest
{
    public string TenDangNhap { get; set; } = null!;
    public string MatKhau { get; set; } = null!;
}

public sealed class TaiKhoanEditRequest
{
    public string? TenDangNhap { get; set; }
    public string? MatKhau { get; set; }
    public string? VaiTro { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class TaiKhoanController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly JwtSigningSettings _jwt;

    public TaiKhoanController(AppDbContext context, JwtSigningSettings jwt)
    {
        _context = context;
        _jwt = jwt;
    }

    // POST: api/TaiKhoan/dang-nhap
    [HttpPost("dang-nhap")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.TenDangNhap) || string.IsNullOrWhiteSpace(request.MatKhau))
            return BadRequest("Tên đăng nhập và mật khẩu không được để trống.");

        var taiKhoan = await _context.TaiKhoans
            .Include(t => t.NhanVien)
            .FirstOrDefaultAsync(t => t.TenDangNhap == request.TenDangNhap);

        if (taiKhoan == null ||
            !PasswordSecurity.Verify(request.MatKhau, taiKhoan.MatKhau, out bool needsUpgrade))
            return Unauthorized("Tên đăng nhập hoặc mật khẩu không chính xác.");
        if (needsUpgrade)
        {
            taiKhoan.MatKhau = PasswordSecurity.Hash(request.MatKhau);
            await _context.SaveChangesAsync();
        }

        // --- BẮT ĐẦU TẠO TOKEN JWT ---
        var securityKey = new SymmetricSecurityKey(_jwt.Key);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Gắn thông tin quyền (VaiTro) vào Token
        var claims = new[] {
            new Claim(JwtRegisteredClaimNames.Sub, taiKhoan.TenDangNhap),
            new Claim("VaiTro", taiKhoan.VaiTro ?? "NhanVien"),
            new Claim(ClaimTypes.Role, taiKhoan.VaiTro ?? "NhanVien"), 
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.Now.AddHours(2), // Token sống được 2 tiếng
            signingCredentials: credentials);
        
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        // --- KẾT THÚC TẠO TOKEN ---

        return Ok(new
        {
            ThongBao = "Đăng nhập thành công!",
            Token = tokenString, // Gửi mã bảo mật về cho Frontend
            taiKhoan.TaiKhoanId,
            taiKhoan.TenDangNhap,
            taiKhoan.VaiTro,
            NhanVienId = taiKhoan.NhanVien?.NhanVienId,
            HoTen = taiKhoan.NhanVien?.HoTen,
            ChucVu = taiKhoan.NhanVien?.ChucVu
        });
    }

    // GET: api/TaiKhoan
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _context.TaiKhoans
            .Select(t => new { t.TaiKhoanId, t.TenDangNhap, t.VaiTro })
            .ToListAsync();
        return Ok(list);
    }

    // GET: api/TaiKhoan/5
    [Authorize(Roles = "Admin")]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var tk = await _context.TaiKhoans.AsNoTracking().Where(t => t.TaiKhoanId == id)
            .Select(t => new { t.TaiKhoanId, t.TenDangNhap, t.VaiTro }).FirstOrDefaultAsync();
        if (tk == null) return NotFound("Không tìm thấy tài khoản.");
        return Ok(tk);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TaiKhoanEditRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.TenDangNhap) ||
            string.IsNullOrWhiteSpace(request.MatKhau) ||
            request.VaiTro is not ("Admin" or "NhanVien"))
            return BadRequest("Thiếu tên đăng nhập/mật khẩu hoặc vai trò không hợp lệ.");
        var username = request.TenDangNhap.Trim();
        if (await _context.TaiKhoans.AnyAsync(t => t.TenDangNhap == username))
            return Conflict("Tên đăng nhập đã tồn tại.");
        var tk = new TaiKhoan { TenDangNhap = username,
            MatKhau = PasswordSecurity.Hash(request.MatKhau), VaiTro = request.VaiTro };
        _context.TaiKhoans.Add(tk);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = tk.TaiKhoanId },
            new { tk.TaiKhoanId, tk.TenDangNhap, tk.VaiTro });
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] TaiKhoanEditRequest request)
    {
        var tk = await _context.TaiKhoans.FindAsync(id);
        if (tk == null) return NotFound("Không tìm thấy tài khoản.");
        if (request.VaiTro is not ("Admin" or "NhanVien"))
            return BadRequest("Vai trò không hợp lệ.");
        tk.VaiTro = request.VaiTro;
        if (!string.IsNullOrWhiteSpace(request.MatKhau))
            tk.MatKhau = PasswordSecurity.Hash(request.MatKhau);
        await _context.SaveChangesAsync();
        return Ok(new { tk.TaiKhoanId, tk.TenDangNhap, tk.VaiTro });
    }

    // DELETE: api/TaiKhoan/5
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var tk = await _context.TaiKhoans.FindAsync(id);
        if (tk == null) return NotFound("Không tìm thấy tài khoản.");

        bool dangLienKet = await _context.NhanViens.AnyAsync(nv => nv.TaiKhoanId == id);
        if (dangLienKet) return BadRequest("Tài khoản đang liên kết với nhân viên, không thể xóa.");

        _context.TaiKhoans.Remove(tk);
        await _context.SaveChangesAsync();
        return Ok("Xóa thành công.");
    }
}