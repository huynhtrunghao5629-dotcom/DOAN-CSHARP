using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhachSanAPI.Models;

namespace QuanLyKhachSanAPI.Controllers;

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
        var tongDoanhThu = await _context.DatPhongs
            .Where(d => d.TrangThai == "DaTra" || d.TrangThai == "DaThanhToan")
            .SumAsync(d => d.TongTien);

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

    // GET: api/ThongKe/doanh-thu-theo-thang?nam=2024
    [HttpGet("doanh-thu-theo-thang")]
    public async Task<IActionResult> GetDoanhThuTheoThang([FromQuery] int nam)
    {
        var thongKe = await _context.DatPhongs
            .Where(d => d.NgayDat.Year == nam && (d.TrangThai == "DaTra" || d.TrangThai == "DaThanhToan"))
            .GroupBy(d => d.NgayDat.Month)
            .Select(g => new
            {
                Thang = g.Key,
                DoanhThu = g.Sum(d => d.TongTien),
                SoDon = g.Count()
            })
            .OrderBy(x => x.Thang)
            .ToListAsync();

        return Ok(thongKe);
    }
}