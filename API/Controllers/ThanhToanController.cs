using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
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

    public sealed class CheckOutRequest
    {
        public string PhuongThuc { get; set; } = "TienMat";
    }

    // Thanh toán và trả phòng đồng thời; một yêu cầu lặp lại sẽ bị từ chối.
    [Authorize(Roles = "Admin,NhanVien")]
    [HttpPost("check-out/{datPhongId:int}")]
    public async Task<IActionResult> CheckOut(int datPhongId, [FromBody] CheckOutRequest? request)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        try
        {
            var don = await _context.DatPhongs
                .Include(d => d.ChiTietDatPhongs).ThenInclude(ct => ct.Phong)
                .Include(d => d.ThanhToans)
                .FirstOrDefaultAsync(d => d.DatPhongId == datPhongId);
            if (don == null) return NotFound("Không tìm thấy đơn đặt phòng.");
            if (don.TrangThai != "DaNhanPhong")
                return Conflict("Chỉ được thanh toán và trả phòng với đơn đã nhận phòng.");
            if (don.ThanhToans.Any())
                return Conflict("Đơn đã có thanh toán, cần xử lý riêng trước khi tiếp tục.");
            if (don.TongTien <= 0)
                return Conflict("Tổng tiền phải lớn hơn 0 theo ràng buộc thanh toán SQL Server.");
            var phuongThuc = (request?.PhuongThuc ?? "TienMat").Trim();
            if (phuongThuc != "TienMat" && phuongThuc != "ChuyenKhoan")
                return BadRequest("Phương thức thanh toán không được hỗ trợ.");

            var thanhToan = new ThanhToan
            {
                DatPhongId = don.DatPhongId,
                NgayThanhToan = DateTime.Now,
                SoTien = don.TongTien,
                PhuongThuc = phuongThuc,
                TrangThai = "DaThanhToan"
            };
            _context.ThanhToans.Add(thanhToan);
            don.TrangThai = "DaTraPhong";
            foreach (var chiTiet in don.ChiTietDatPhongs)
            {
                // Không xóa lịch đặt tương lai khác, chỉ cập nhật hiện trạng phòng.
                if (chiTiet.Phong != null)
                {
                    var coLichKhac = await _context.ChiTietDatPhongs.AnyAsync(ct =>
                        ct.PhongId == chiTiet.PhongId && ct.DatPhongId != don.DatPhongId &&
                        ct.DatPhong.TrangThai != "DaHuy" &&
                        ct.DatPhong.TrangThai != "DaTraPhong");
                    chiTiet.Phong.TrangThai = coLichKhac ? "DaDat" : "Trong";
                }
            }
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return Ok(new { ThongBao = "Thanh toán và trả phòng thành công.",
                don.DatPhongId, don.TrangThai, thanhToan.ThanhToanId,
                thanhToan.SoTien });
        }
        catch (DbUpdateException)
        {
            return Conflict("Không thể lưu thanh toán. Kiểm tra quy định SQL Server.");
        }
        catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 1205)
        {
            return Conflict("Giao dịch đang được xử lý. Vui lòng kiểm tra lại trạng thái đơn.");
        }
    }

    [Authorize(Roles = "Admin,NhanVien")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _context.ThanhToans.ToListAsync());
    }

    [Authorize(Roles = "Admin,NhanVien")]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var tt = await _context.ThanhToans.FindAsync(id);
        if (tt == null) return NotFound("Không tìm thấy thanh toán.");
        return Ok(tt);
    }

    [Authorize(Roles = "Admin,NhanVien")]
    [HttpPost]
    public IActionResult Create(ThanhToan model) =>
        Conflict("Sử dụng POST api/ThanhToan/check-out/{datPhongId} để lưu và trả phòng trong cùng giao dịch.");

    [Authorize(Roles = "Admin,NhanVien")]
    [HttpPut("{id:int}")]
    public IActionResult Update(int id, ThanhToan model) =>
        Conflict("Không cho sửa giao dịch thanh toán đã ghi nhận.");

    [Authorize(Roles = "Admin,NhanVien")]
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id) =>
        Conflict("Không cho xóa chứng từ thanh toán; cần lập nghiệp vụ hoàn tiền riêng.");
}
