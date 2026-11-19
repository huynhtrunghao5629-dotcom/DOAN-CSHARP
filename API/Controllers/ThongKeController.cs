using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhachSanAPI.Models;

namespace QuanLyKhachSanAPI.Controllers;

[Authorize(Roles = "Admin,NhanVien")]
[ApiController]
[Route("api/[controller]")]
public class ThongKeController : ControllerBase
{
    private readonly AppDbContext _context;

    public ThongKeController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/ThongKe/tong-quan
    [HttpGet("tong-quan")]
    public async Task<IActionResult> GetTongQuan()
    {
        var tongSoPhong = await _context.Phongs.CountAsync();
        var tongKhachHang = await _context.KhachHangs.CountAsync();
        var tongDonDat = await _context.DatPhongs.CountAsync();
        
        // Tính tổng doanh thu từ các đơn đã thanh toán/đã trả phòng
        var tongDoanhThu = await _context.ThanhToans
            .Where(t => t.DatPhong.TrangThai == "DaTraPhong")
            .SumAsync(t => (decimal?)t.SoTien) ?? 0m;

        // Đếm số phòng đang trống
        var soPhongTrong = await _context.Phongs.CountAsync(p => p.TrangThai == "Trong");

        return Ok(new
        {
            TongSoPhong = tongSoPhong,
            SoPhongTrong = soPhongTrong,
            TongKhachHang = tongKhachHang,
            TongDonDatPhong = tongDonDat,
            TongDoanhThu = tongDoanhThu
        });
    }

    // Doanh thu theo tháng thanh toán; số đơn theo tháng tạo đơn.
    [HttpGet("doanh-thu-theo-thang")]
    public async Task<IActionResult> GetDoanhThuTheoThang([FromQuery] int nam)
    {
        if (nam < 2000 || nam > 2100) return BadRequest("Năm không hợp lệ.");
        var soDon = await _context.DatPhongs
            .Where(d => d.NgayDat.Year == nam)
            .GroupBy(d => d.NgayDat.Month)
            .Select(g => new { Thang = g.Key, SoDon = g.Count() })
            .ToListAsync();
        var thuThucTe = await _context.ThanhToans
            .Where(t => t.DatPhong.TrangThai == "DaTraPhong" && t.NgayThanhToan.Year == nam)
            .GroupBy(t => t.NgayThanhToan.Month)
            .Select(g => new { Thang = g.Key, DoanhThu = g.Sum(t => t.SoTien) })
            .ToListAsync();
        var result = Enumerable.Range(1, 12).Select(thang => new
        {
            Thang = thang,
            SoDon = soDon.FirstOrDefault(x => x.Thang == thang)?.SoDon ?? 0,
            DoanhThu = thuThucTe.FirstOrDefault(x => x.Thang == thang)?.DoanhThu ?? 0m
        });
        return Ok(result);
    }
}
