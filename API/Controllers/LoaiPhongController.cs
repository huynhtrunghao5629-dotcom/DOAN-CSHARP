using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhachSanAPI.Models;

namespace QuanLyKhachSanAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoaiPhongController : ControllerBase
{
    private readonly AppDbContext _context;

    public LoaiPhongController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/LoaiPhong
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var loaiPhongs = await _context.LoaiPhongs.ToListAsync();
        return Ok(loaiPhongs);
    }

    // GET: api/LoaiPhong/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var loaiPhong = await _context.LoaiPhongs.FindAsync(id);
        if (loaiPhong == null)
            return NotFound("Không tìm thấy loại phòng.");

        return Ok(loaiPhong);
    }

    // POST: api/LoaiPhong
    [HttpPost]
    public async Task<IActionResult> Create(LoaiPhong loaiPhong)
    {
        if (string.IsNullOrWhiteSpace(loaiPhong.TenLoai))
            return BadRequest("Tên loại phòng không được để trống.");

        bool trungTen = await _context.LoaiPhongs.AnyAsync(lp => lp.TenLoai == loaiPhong.TenLoai);
        if (trungTen)
            return BadRequest("Tên loại phòng đã tồn tại.");

        loaiPhong.LoaiPhongId = 0;
        _context.LoaiPhongs.Add(loaiPhong);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = loaiPhong.LoaiPhongId }, loaiPhong);
    }

    // PUT: api/LoaiPhong/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] LoaiPhong loaiPhong)
    {
        var loaiPhongCu = await _context.LoaiPhongs.FindAsync(id);
        if (loaiPhongCu == null)
            return NotFound("Không tìm thấy loại phòng.");

        if (string.IsNullOrWhiteSpace(loaiPhong.TenLoai))
            return BadRequest("Tên loại phòng không được để trống.");

        // Kiểm tra trùng tên loại phòng với CÁC LOẠI PHÒNG KHÁC (khác id hiện tại)
        bool trungTen = await _context.LoaiPhongs.AnyAsync(lp => lp.TenLoai == loaiPhong.TenLoai && lp.LoaiPhongId != id);
        if (trungTen)
            return BadRequest("Tên loại phòng đã tồn tại.");

        // Cập nhật thông tin chuẩn
        loaiPhongCu.TenLoai = loaiPhong.TenLoai;
        loaiPhongCu.GiaPhong = loaiPhong.GiaPhong;

        await _context.SaveChangesAsync();
        return Ok(loaiPhongCu);
    }

    // DELETE: api/LoaiPhong/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var loaiPhong = await _context.LoaiPhongs.FindAsync(id);
        if (loaiPhong == null)
            return NotFound("Không tìm thấy loại phòng.");

        bool daCoPhong = await _context.Phongs.AnyAsync(p => p.LoaiPhongId == id);
        if (daCoPhong)
            return BadRequest("Loại phòng này đang có phòng liên kết, không thể xóa.");

        _context.LoaiPhongs.Remove(loaiPhong);
        await _context.SaveChangesAsync();
        return Ok("Xóa loại phòng thành công.");
    }
}