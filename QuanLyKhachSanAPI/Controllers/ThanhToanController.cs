using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhachSanAPI.Models;

namespace QuanLyKhachSanAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ThanhToanController : ControllerBase
{
    private readonly AppDbContext _context;

    public ThanhToanController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _context.ThanhToans.ToListAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var tt = await _context.ThanhToans.FindAsync(id);
        if (tt == null) return NotFound("Không tìm thấy thanh toán.");
        return Ok(tt);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ThanhToan model)
    {
        var datPhong = await _context.DatPhongs.FindAsync(model.DatPhongId);
        if (datPhong == null) return BadRequest("Đơn đặt phòng không tồn tại.");

        model.ThanhToanId = 0;
        model.NgayThanhToan = DateTime.Now;
        
        _context.ThanhToans.Add(model);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = model.ThanhToanId }, model);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ThanhToan model)
    {
        var tt = await _context.ThanhToans.FindAsync(id);
        if (tt == null) return NotFound("Không tìm thấy hóa đơn thanh toán.");

        tt.SoTien = model.SoTien;
        tt.PhuongThuc = model.PhuongThuc;
        tt.TrangThai = model.TrangThai;

        await _context.SaveChangesAsync();
        return Ok(tt);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var tt = await _context.ThanhToans.FindAsync(id);
        if (tt == null) return NotFound("Không tìm thấy hóa đơn.");

        _context.ThanhToans.Remove(tt);
        await _context.SaveChangesAsync();
        return Ok("Xóa thành công.");
    }
}