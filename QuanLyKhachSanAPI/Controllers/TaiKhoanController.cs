using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhachSanAPI.Models;

namespace QuanLyKhachSanAPI.Controllers;

public class LoginRequest
{
    public string TenDangNhap { get; set; } = null!;
    public string MatKhau { get; set; } = null!;
}

[ApiController]
[Route("api/[controller]")]
public class TaiKhoanController : ControllerBase
{
    private readonly AppDbContext _context;

    public TaiKhoanController(AppDbContext context)
    {
        _context = context;
    }

    // POST: api/TaiKhoan/dang-nhap
    [HttpPost("dang-nhap")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.TenDangNhap) || string.IsNullOrWhiteSpace(request.MatKhau))
            return BadRequest("Tên đăng nhập và mật khẩu không được để trống.");

        var taiKhoan = await _context.TaiKhoans
            .Include(t => t.NhanVien)
            .FirstOrDefaultAsync(t => t.TenDangNhap == request.TenDangNhap && t.MatKhau == request.MatKhau);

        if (taiKhoan == null)
            return Unauthorized("Tên đăng nhập hoặc mật khẩu không chính xác.");

        return Ok(new
        {
            taiKhoan.TaiKhoanId,
            taiKhoan.TenDangNhap,
            taiKhoan.VaiTro,
            NhanVienId = taiKhoan.NhanVien?.NhanVienId,
            HoTen = taiKhoan.NhanVien?.HoTen,
            ChucVu = taiKhoan.NhanVien?.ChucVu
        });
    }

    // GET: api/TaiKhoan
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _context.TaiKhoans
            .Select(t => new { t.TaiKhoanId, t.TenDangNhap, t.VaiTro })
            .ToListAsync();
        return Ok(list);
    }

    // GET: api/TaiKhoan/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var tk = await _context.TaiKhoans.FindAsync(id);
        if (tk == null) return NotFound("Không tìm thấy tài khoản.");
        return Ok(tk);
    }

    // POST: api/TaiKhoan
    [HttpPost]
    public async Task<IActionResult> Create(TaiKhoan model)
    {
        bool tonTai = await _context.TaiKhoans.AnyAsync(t => t.TenDangNhap == model.TenDangNhap);
        if (tonTai) return BadRequest("Tên đăng nhập đã tồn tại.");

        model.TaiKhoanId = 0;
        _context.TaiKhoans.Add(model);
        await _context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetById), new { id = model.TaiKhoanId }, model);
    }

    // PUT: api/TaiKhoan/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, TaiKhoan model)
    {
        var tk = await _context.TaiKhoans.FindAsync(id);
        if (tk == null) return NotFound("Không tìm thấy tài khoản.");

        tk.MatKhau = model.MatKhau;
        tk.VaiTro = model.VaiTro;
        
        await _context.SaveChangesAsync();
        return Ok(tk);
    }

    // DELETE: api/TaiKhoan/5
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