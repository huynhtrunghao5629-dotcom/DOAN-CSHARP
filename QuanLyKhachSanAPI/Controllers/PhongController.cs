using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhachSanAPI.Models;

namespace QuanLyKhachSanAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PhongController : ControllerBase
{
    private readonly AppDbContext _context;

    public PhongController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/Phong
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var phong = await _context.Phongs
            .Include(p => p.LoaiPhong)
            .Select(p => new
            {
                p.PhongId,
                p.SoPhong,
                p.TrangThai,
                p.LoaiPhongId,
                LoaiPhong = p.LoaiPhong.TenLoai,
                GiaPhong = p.LoaiPhong.GiaPhong
            })
            .ToListAsync();

        return Ok(phong);
    }

    // GET: api/Phong/phan-trang (TÍNH NĂNG MỚI THÊM VÀO)
    [HttpGet("phan-trang")]
    public async Task<IActionResult> GetPhongsPhanTrang([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var query = _context.Phongs.Include(p => p.LoaiPhong);
        var totalRecords = await query.CountAsync();
        
        var phongs = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new
            {
                p.PhongId,
                p.SoPhong,
                p.TrangThai,
                p.LoaiPhongId,
                LoaiPhong = p.LoaiPhong.TenLoai,
                GiaPhong = p.LoaiPhong.GiaPhong
            })
            .ToListAsync();

        return Ok(new
        {
            TongSoPhong = totalRecords,
            TrangHienTai = page,
            KichThuocTrang = pageSize,
            TongSoTrang = (int)Math.Ceiling((double)totalRecords / pageSize),
            DuLieu = phongs
        });
    }

    // GET: api/Phong/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var phong = await _context.Phongs
            .Include(p => p.LoaiPhong)
            .Where(p => p.PhongId == id)
            .Select(p => new
            {
                p.PhongId,
                p.SoPhong,
                p.TrangThai,
                p.LoaiPhongId,
                LoaiPhong = p.LoaiPhong.TenLoai,
                GiaPhong = p.LoaiPhong.GiaPhong
            })
            .FirstOrDefaultAsync();

        if (phong == null)
            return NotFound("Khong tim thay phong.");

        return Ok(phong);
    }

    // GET: api/Phong/loai/1
    [HttpGet("loai/{loaiPhongId:int}")]
    public async Task<IActionResult> GetByLoai(int loaiPhongId)
    {
        var phong = await _context.Phongs
            .Include(p => p.LoaiPhong)
            .Where(p => p.LoaiPhongId == loaiPhongId)
            .Select(p => new
            {
                p.PhongId,
                p.SoPhong,
                p.TrangThai,
                LoaiPhong = p.LoaiPhong.TenLoai,
                GiaPhong = p.LoaiPhong.GiaPhong
            })
            .ToListAsync();

        return Ok(phong);
    }

    // GET: api/Phong/trong
    [HttpGet("trong")]
    public async Task<IActionResult> GetPhongTrong()
    {
        var phong = await _context.Phongs
            .Include(p => p.LoaiPhong)
            .Where(p => p.TrangThai == "Trong")
            .Select(p => new
            {
                p.PhongId,
                p.SoPhong,
                p.TrangThai,
                LoaiPhong = p.LoaiPhong.TenLoai,
                GiaPhong = p.LoaiPhong.GiaPhong
            })
            .ToListAsync();

        return Ok(phong);
    }

    // GET: api/Phong/tim-kiem
    [HttpGet("tim-kiem")]
    public async Task<IActionResult> TimKiemPhong([FromQuery] DateOnly? ngayNhan, [FromQuery] DateOnly? ngayTra, [FromQuery] decimal? giaTu, [FromQuery] decimal? giaDen)
    {
        var query = _context.Phongs.Include(p => p.LoaiPhong).AsQueryable();

        if (giaTu.HasValue)
            query = query.Where(p => p.LoaiPhong.GiaPhong >= giaTu.Value);
        
        if (giaDen.HasValue)
            query = query.Where(p => p.LoaiPhong.GiaPhong <= giaDen.Value);

        if (ngayNhan.HasValue && ngayTra.HasValue)
        {
            if (ngayTra.Value <= ngayNhan.Value)
                return BadRequest("Ngày trả phòng phải sau ngày nhận phòng.");

            var danhSachPhongBiDat = await _context.ChiTietDatPhongs
                .Include(ct => ct.DatPhong)
                .Where(ct => 
                    ct.DatPhong.TrangThai != "DaHuy" && 
                    ct.DatPhong.TrangThai != "DaTra" && 
                    ct.DatPhong.NgayNhan < ngayTra.Value && 
                    ct.DatPhong.NgayTra > ngayNhan.Value)
                .Select(ct => ct.PhongId)
                .Distinct()
                .ToListAsync();

            if (danhSachPhongBiDat.Any())
            {
                query = query.Where(p => !danhSachPhongBiDat.Contains(p.PhongId));
            }
        }

        var ketQua = await query.Select(p => new
        {
            p.PhongId,
            p.SoPhong,
            p.TrangThai,
            p.LoaiPhongId,
            TenLoai = p.LoaiPhong.TenLoai,
            GiaPhong = p.LoaiPhong.GiaPhong
        }).ToListAsync();

        return Ok(ketQua);
    }

    // POST: api/Phong
    [HttpPost]
    public async Task<IActionResult> Create(Phong phong)
    {
        if (string.IsNullOrWhiteSpace(phong.SoPhong))
            return BadRequest("So phong khong duoc de trong.");

        bool trungSoPhong = await _context.Phongs.AnyAsync(p => p.SoPhong == phong.SoPhong);
        if (trungSoPhong) return BadRequest("So phong da ton tai.");

        bool loaiPhongTonTai = await _context.LoaiPhongs.AnyAsync(lp => lp.LoaiPhongId == phong.LoaiPhongId);
        if (!loaiPhongTonTai) return BadRequest("Loai phong khong ton tai.");

        phong.PhongId = 0;
        _context.Phongs.Add(phong);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = phong.PhongId }, phong);
    }

    // PUT: api/Phong/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Phong phong)
    {
        var phongCu = await _context.Phongs.FindAsync(id);
        if (phongCu == null) return NotFound("Khong tim thay phong.");

        if (string.IsNullOrWhiteSpace(phong.SoPhong))
            return BadRequest("So phong khong duoc de trong.");

        bool trungSoPhong = await _context.Phongs.AnyAsync(p => p.SoPhong == phong.SoPhong && p.PhongId != id);
        if (trungSoPhong) return BadRequest("So phong da ton tai.");

        bool loaiPhongTonTai = await _context.LoaiPhongs.AnyAsync(lp => lp.LoaiPhongId == phong.LoaiPhongId);
        if (!loaiPhongTonTai) return BadRequest("Loai phong khong ton tai.");

        phongCu.SoPhong = phong.SoPhong;
        phongCu.LoaiPhongId = phong.LoaiPhongId;
        phongCu.TrangThai = phong.TrangThai;

        await _context.SaveChangesAsync();
        return Ok(phongCu);
    }

    // DELETE: api/Phong/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var phong = await _context.Phongs.FindAsync(id);
        if (phong == null) return NotFound("Khong tim thay phong.");

        bool dangDuocDat = await _context.ChiTietDatPhongs.AnyAsync(ct => ct.PhongId == id);
        if (dangDuocDat) return BadRequest("Phong da co lich dat, khong the xoa.");

        _context.Phongs.Remove(phong);
        await _context.SaveChangesAsync();

        return Ok("Xoa phong thanh cong.");
    }
}