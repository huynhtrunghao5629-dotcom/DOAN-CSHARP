using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhachSanAPI.Models;

namespace QuanLyKhachSanAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DichVuController : ControllerBase
{
    private readonly AppDbContext _context;
    public DichVuController(AppDbContext context) => _context = context;

    // Chỉ đọc danh mục dịch vụ. Chưa mở chức năng ghi/xóa trên WinForms.
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var ds = await _context.DichVus
            .AsNoTracking()
            .OrderBy(x => x.TenDichVu)
            .Select(x => new { x.DichVuId, x.TenDichVu, x.DonGia, x.DonViTinh, x.MoTa })
            .ToListAsync();
        return Ok(ds);
    }
}
