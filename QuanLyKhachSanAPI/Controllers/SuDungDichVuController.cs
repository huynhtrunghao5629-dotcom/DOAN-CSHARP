using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhachSanAPI.Models;

namespace QuanLyKhachSanAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SuDungDichVuController : ControllerBase
{
    private readonly AppDbContext _context;

    public SuDungDichVuController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _context.SuDungDichVus
            .Include(s => s.DichVu)
            .Select(s => new {
                s.SuDungDichVuId, s.DatPhongId, s.DichVuId, s.SoLuong, s.DonGia, s.NgaySuDung,
                TenDichVu = s.DichVu.TenDichVu
            }).ToListAsync();
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var s = await _context.SuDungDichVus.FindAsync(id);
        if (s == null) return NotFound("Không tìm thấy chi tiết sử dụng dịch vụ.");
        return Ok(s);
    }

    [HttpGet("dat-phong/{datPhongId:int}")]
    public async Task<IActionResult> GetByDatPhong(int datPhongId)
    {
        var list = await _context.SuDungDichVus
            .Include(s => s.DichVu)
            .Where(s => s.DatPhongId == datPhongId)
            .Select(s => new {
                s.SuDungDichVuId, s.DichVuId, s.SoLuong, s.DonGia, s.NgaySuDung,
                TenDichVu = s.DichVu.TenDichVu,
                ThanhTien = s.SoLuong * s.DonGia
            }).ToListAsync();
        return Ok(list);
    }

    [HttpPost]
    public async Task<IActionResult> Create(SuDungDichVu model)
    {
        var dichVu = await _context.DichVus.FindAsync(model.DichVuId);
        if (dichVu == null) return BadRequest("Dịch vụ không tồn tại.");

        var datPhong = await _context.DatPhongs.FindAsync(model.DatPhongId);
        if (datPhong == null) return BadRequest("Đơn đặt phòng không tồn tại.");

        model.SuDungDichVuId = 0;
        model.NgaySuDung = DateTime.Now;
        model.DonGia = dichVu.DonGia; // Lấy giá hiện tại của dịch vụ
        
        _context.SuDungDichVus.Add(model);

        // Cập nhật tổng tiền đơn đặt phòng
        datPhong.TongTien += (model.DonGia * model.SoLuong);

        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = model.SuDungDichVuId }, model);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, SuDungDichVu model)
    {
        var s = await _context.SuDungDichVus.FindAsync(id);
        if (s == null) return NotFound("Không tìm thấy chi tiết sử dụng dịch vụ.");

        var datPhong = await _context.DatPhongs.FindAsync(s.DatPhongId);
        if (datPhong != null)
        {
            // Trừ tiền cũ, cộng tiền mới
            datPhong.TongTien -= (s.DonGia * s.SoLuong);
            datPhong.TongTien += (model.DonGia * model.SoLuong);
        }

        s.SoLuong = model.SoLuong;
        s.DonGia = model.DonGia;

        await _context.SaveChangesAsync();
        return Ok(s);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var s = await _context.SuDungDichVus.FindAsync(id);
        if (s == null) return NotFound("Không tìm thấy dữ liệu.");

        var datPhong = await _context.DatPhongs.FindAsync(s.DatPhongId);
        if (datPhong != null)
        {
            datPhong.TongTien -= (s.DonGia * s.SoLuong); // Hoàn lại tổng tiền
        }

        _context.SuDungDichVus.Remove(s);
        await _context.SaveChangesAsync();
        return Ok("Xóa thành công.");
    }
}