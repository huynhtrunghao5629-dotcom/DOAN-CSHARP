using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhachSanAPI.Models;

namespace QuanLyKhachSanAPI.Controllers;

public sealed class SuDungDichVuRequest
{
    public int DatPhongId { get; set; }
    public int DichVuId { get; set; }
    public int SoLuong { get; set; }
}

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
        return Ok(new { s.SuDungDichVuId, s.DatPhongId, s.DichVuId,
            s.SoLuong, s.DonGia, s.NgaySuDung });
    }

    [HttpGet("dat-phong/{datPhongId:int}")]
    public async Task<IActionResult> GetByDatPhong(int datPhongId)
    {
        var list = await _context.SuDungDichVus
            .Include(s => s.DichVu)
            .Where(s => s.DatPhongId == datPhongId)
            .Select(s => new {
                s.SuDungDichVuId, s.DatPhongId, s.DichVuId, s.SoLuong, s.DonGia, s.NgaySuDung,
                TenDichVu = s.DichVu.TenDichVu,
                ThanhTien = s.SoLuong * s.DonGia
            }).ToListAsync();
        return Ok(list);
    }

    [Authorize(Roles = "Admin,NhanVien")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SuDungDichVuRequest request)
    {
        var dichVu = await _context.DichVus.FindAsync(request.DichVuId);
        if (dichVu == null) return BadRequest("Dịch vụ không tồn tại.");

        var datPhong = await _context.DatPhongs.FindAsync(request.DatPhongId);
        if (datPhong == null) return BadRequest("Đơn đặt phòng không tồn tại.");
        if (datPhong.TrangThai != "DaNhanPhong")
            return Conflict("Chỉ thêm dịch vụ cho đơn đang nhận phòng.");
        if (request.SoLuong <= 0) return BadRequest("Số lượng dịch vụ phải lớn hơn 0.");
        var model = new SuDungDichVu
        {
            DatPhongId = request.DatPhongId,
            DichVuId = request.DichVuId,
            SoLuong = request.SoLuong,
            DonGia = dichVu.DonGia,
            NgaySuDung = DateTime.Now
        };
        _context.SuDungDichVus.Add(model);
        datPhong.TongTien += model.DonGia * model.SoLuong;
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = model.SuDungDichVuId },
            new { model.SuDungDichVuId, model.DatPhongId, model.DichVuId,
                model.SoLuong, model.DonGia, model.NgaySuDung });
    }

    [Authorize(Roles = "Admin,NhanVien")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] SuDungDichVuRequest request)
    {
        var s = await _context.SuDungDichVus.FindAsync(id);
        if (s == null) return NotFound("Không tìm thấy chi tiết sử dụng dịch vụ.");
        if (request.SoLuong <= 0) return BadRequest("Số lượng phải lớn hơn 0.");
        if (!await _context.DatPhongs.AnyAsync(d => d.DatPhongId == s.DatPhongId && d.TrangThai == "DaNhanPhong"))
            return Conflict("Không sửa dịch vụ của đơn đã hoàn tất.");

        var datPhong = await _context.DatPhongs.FindAsync(s.DatPhongId);
        if (datPhong != null)
        {
            // Trừ tiền cũ, cộng tiền mới
            datPhong.TongTien -= (s.DonGia * s.SoLuong);
            datPhong.TongTien += (s.DonGia * request.SoLuong);
        }

        s.SoLuong = request.SoLuong;
        // Không tin đơn giá do client tự gửi.
        // Đơn giá đã được chốt tại lúc thêm dịch vụ.

        await _context.SaveChangesAsync();
        // Không trả entity EF trực tiếp: s.DatPhong.SuDungDichVus có thể tạo vòng lặp JSON.
        return Ok(new
        {
            s.SuDungDichVuId,
            s.DatPhongId,
            s.DichVuId,
            s.SoLuong,
            s.DonGia,
            s.NgaySuDung,
            ThanhTien = s.SoLuong * s.DonGia
        });
    }

    [Authorize(Roles = "Admin,NhanVien")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var s = await _context.SuDungDichVus.FindAsync(id);
        if (s == null) return NotFound("Không tìm thấy dữ liệu.");
        if (!await _context.DatPhongs.AnyAsync(d => d.DatPhongId == s.DatPhongId && d.TrangThai == "DaNhanPhong"))
            return Conflict("Không xóa dịch vụ của đơn đã hoàn tất.");

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