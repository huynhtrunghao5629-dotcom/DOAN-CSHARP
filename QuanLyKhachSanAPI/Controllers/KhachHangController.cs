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

    // GET: api/KhachHang/tim-kiem?tuKhoa=...
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

        if (!string.IsNullOrWhiteSpace(khachHang.Cccd))
        {
            bool trungCccd = await _context.KhachHangs.AnyAsync(k => k.Cccd == khachHang.Cccd);
            if (trungCccd)
                return BadRequest("Số CCCD đã tồn tại trong hệ thống.");
        }

        khachHang.KhachHangId = 0;
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
}