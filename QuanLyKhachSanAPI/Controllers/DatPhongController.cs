using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhachSanAPI.Models;

namespace QuanLyKhachSanAPI.Controllers;

public class TaoDatPhongRequest
{
    public int KhachHangId { get; set; }
    public int? NhanVienId { get; set; }
    public DateOnly NgayNhan { get; set; }
    public DateOnly NgayTra { get; set; }
    public List<int> DanhSachPhongId { get; set; } = new();
}

public class CapNhatTrangThaiRequest
{
    public string TrangThai { get; set; } = null!;
}

[ApiController]
[Route("api/[controller]")]
public class DatPhongController : ControllerBase
{
    private readonly AppDbContext _context;

    public DatPhongController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/DatPhong
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var danhSach = await _context.DatPhongs
            .Include(d => d.KhachHang)
            .Include(d => d.ChiTietDatPhongs)
                .ThenInclude(ct => ct.Phong)
            .Select(d => new
            {
                d.DatPhongId,
                d.KhachHangId,
                TenKhachHang = d.KhachHang.HoTen,
                SoDienThoai = d.KhachHang.SoDienThoai,
                d.NhanVienId,
                d.NgayDat,
                d.NgayNhan,
                d.NgayTra,
                d.TrangThai,
                d.TongTien,
                DanhSachPhong = d.ChiTietDatPhongs.Select(ct => new
                {
                    ct.PhongId,
                    ct.Phong.SoPhong,
                    ct.GiaPhong
                })
            })
            .OrderByDescending(d => d.NgayDat)
            .ToListAsync();

        return Ok(danhSach);
    }

    // GET: api/DatPhong/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var datPhong = await _context.DatPhongs
            .Include(d => d.KhachHang)
            .Include(d => d.ChiTietDatPhongs)
                .ThenInclude(ct => ct.Phong)
            .Where(d => d.DatPhongId == id)
            .Select(d => new
            {
                d.DatPhongId,
                d.KhachHangId,
                TenKhachHang = d.KhachHang.HoTen,
                d.KhachHang.SoDienThoai,
                d.KhachHang.Cccd,
                d.NhanVienId,
                d.NgayDat,
                d.NgayNhan,
                d.NgayTra,
                d.TrangThai,
                d.TongTien,
                ChiTietPhong = d.ChiTietDatPhongs.Select(ct => new
                {
                    ct.PhongId,
                    ct.Phong.SoPhong,
                    ct.GiaPhong
                })
            })
            .FirstOrDefaultAsync();

        if (datPhong == null)
            return NotFound("Không tìm thấy đơn đặt phòng.");

        return Ok(datPhong);
    }

    // POST: api/DatPhong
    [HttpPost]
    public async Task<IActionResult> Create(TaoDatPhongRequest request)
    {
        if (request.NgayTra <= request.NgayNhan)
            return BadRequest("Ngày trả phòng phải sau ngày nhận phòng.");

        if (request.DanhSachPhongId == null || !request.DanhSachPhongId.Any())
            return BadRequest("Vui lòng chọn ít nhất một phòng.");

        var khachHang = await _context.KhachHangs.FindAsync(request.KhachHangId);
        if (khachHang == null)
            return BadRequest("Khách hàng không tồn tại.");

        var phongs = await _context.Phongs
            .Include(p => p.LoaiPhong)
            .Where(p => request.DanhSachPhongId.Contains(p.PhongId))
            .ToListAsync();

        if (phongs.Count != request.DanhSachPhongId.Count)
            return BadRequest("Một số phòng chọn không tồn tại.");

        // Tính số ngày thuê
        int soNgay = request.NgayTra.DayNumber - request.NgayNhan.DayNumber;
        decimal tongTien = phongs.Sum(p => p.LoaiPhong.GiaPhong) * soNgay;

        var datPhong = new DatPhong
        {
            KhachHangId = request.KhachHangId,
            NhanVienId = request.NhanVienId,
            NgayDat = DateTime.Now,
            NgayNhan = request.NgayNhan,
            NgayTra = request.NgayTra,
            TrangThai = "DaDat",
            TongTien = tongTien
        };

        foreach (var phong in phongs)
        {
            datPhong.ChiTietDatPhongs.Add(new ChiTietDatPhong
            {
                PhongId = phong.PhongId,
                GiaPhong = phong.LoaiPhong.GiaPhong
            });
            phong.TrangThai = "DaDat"; // Cập nhật trạng thái phòng
        }

        _context.DatPhongs.Add(datPhong);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = datPhong.DatPhongId }, new
        {
            datPhong.DatPhongId,
            datPhong.TongTien,
            datPhong.TrangThai,
            ThongBao = "Đặt phòng thành công."
        });
    }

    // PUT: api/DatPhong/5/trang-thai
    [HttpPut("{id:int}/trang-thai")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] CapNhatTrangThaiRequest request)
    {
        var datPhong = await _context.DatPhongs
            .Include(d => d.ChiTietDatPhongs)
                .ThenInclude(ct => ct.Phong)
            .FirstOrDefaultAsync(d => d.DatPhongId == id);

        if (datPhong == null)
            return NotFound("Không tìm thấy đơn đặt phòng.");

        datPhong.TrangThai = request.TrangThai;

        // Tự động giải phóng trạng thái phòng nếu hoàn thành hoặc hủy
        if (request.TrangThai == "DaTra" || request.TrangThai == "DaHuy")
        {
            foreach (var ct in datPhong.ChiTietDatPhongs)
            {
                ct.Phong.TrangThai = "Trong";
            }
        }
        else if (request.TrangThai == "DangO")
        {
            foreach (var ct in datPhong.ChiTietDatPhongs)
            {
                ct.Phong.TrangThai = "DangO";
            }
        }

        await _context.SaveChangesAsync();
        return Ok(new { Message = "Cập nhật trạng thái thành công.", TrangThai = datPhong.TrangThai });
    }

    // PUT: api/DatPhong/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, DatPhong model)
    {
        var dp = await _context.DatPhongs.FindAsync(id);
        if (dp == null) return NotFound("Không tìm thấy đơn đặt phòng.");

        if (model.NgayTra <= model.NgayNhan)
            return BadRequest("Ngày trả phải sau ngày nhận.");

        dp.KhachHangId = model.KhachHangId;
        dp.NhanVienId = model.NhanVienId;
        dp.NgayNhan = model.NgayNhan;
        dp.NgayTra = model.NgayTra;
        dp.TongTien = model.TongTien;

        await _context.SaveChangesAsync();
        return Ok(dp);
    }

    // DELETE: api/DatPhong/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var dp = await _context.DatPhongs
            .Include(d => d.ChiTietDatPhongs)
            .Include(d => d.SuDungDichVus)
            .Include(d => d.ThanhToans)
            .FirstOrDefaultAsync(d => d.DatPhongId == id);

        if (dp == null) return NotFound("Không tìm thấy đơn đặt phòng.");

        // Chuyển trạng thái các phòng đang được đặt về 'Trong'
        foreach (var ct in dp.ChiTietDatPhongs)
        {
            var phong = await _context.Phongs.FindAsync(ct.PhongId);
            if (phong != null) phong.TrangThai = "Trong";
        }

        // Cần xóa các bản ghi phụ thuộc trước khi xóa DatPhong
        _context.SuDungDichVus.RemoveRange(dp.SuDungDichVus);
        _context.ThanhToans.RemoveRange(dp.ThanhToans);
        _context.ChiTietDatPhongs.RemoveRange(dp.ChiTietDatPhongs);
        _context.DatPhongs.Remove(dp);

        await _context.SaveChangesAsync();
        return Ok("Xóa đơn đặt phòng thành công.");
    }
}