using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhachSanAPI.Models;

namespace QuanLyKhachSanAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChiTietDatPhongController : ControllerBase
{
    private readonly AppDbContext _context;

    public ChiTietDatPhongController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _context.ChiTietDatPhongs.ToListAsync();
        return Ok(list);
    }

    [HttpGet("{datPhongId:int}/{phongId:int}")]
    public async Task<IActionResult> GetById(int datPhongId, int phongId)
    {
        var ct = await _context.ChiTietDatPhongs
            .FirstOrDefaultAsync(c => c.DatPhongId == datPhongId && c.PhongId == phongId);
        if (ct == null) return NotFound("Không tìm thấy chi tiết đặt phòng.");
        return Ok(ct);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ChiTietDatPhong model)
    {
        var tonTai = await _context.ChiTietDatPhongs
            .AnyAsync(c => c.DatPhongId == model.DatPhongId && c.PhongId == model.PhongId);
        if (tonTai) return BadRequest("Phòng này đã được thêm vào đơn đặt phòng.");

        _context.ChiTietDatPhongs.Add(model);
        await _context.SaveChangesAsync();
        return Ok(model);
    }

    [HttpPut("{datPhongId:int}/{phongId:int}")]
    public async Task<IActionResult> Update(int datPhongId, int phongId, ChiTietDatPhong model)
    {
        var ct = await _context.ChiTietDatPhongs
            .FirstOrDefaultAsync(c => c.DatPhongId == datPhongId && c.PhongId == phongId);
        if (ct == null) return NotFound();

        ct.GiaPhong = model.GiaPhong;
        await _context.SaveChangesAsync();
        return Ok(ct);
    }

    [HttpDelete("{datPhongId:int}/{phongId:int}")]
    public async Task<IActionResult> Delete(int datPhongId, int phongId)
    {
        var ct = await _context.ChiTietDatPhongs
            .FirstOrDefaultAsync(c => c.DatPhongId == datPhongId && c.PhongId == phongId);
        if (ct == null) return NotFound();

        _context.ChiTietDatPhongs.Remove(ct);
        await _context.SaveChangesAsync();
        return Ok("Xóa thành công.");
    }
}