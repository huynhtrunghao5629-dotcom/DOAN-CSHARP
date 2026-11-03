using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhachSanAPI.Models;

namespace QuanLyKhachSanAPI.Controllers;

public class TaoDatPhongRequest
{
    public int KhachHangId { get; set; }
    public int? NhanVienId { get; set; }
    public DateTime? NgayNhan { get; set; }
    public DateTime? NgayTra { get; set; }
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
        // Tính số ngày thuê (Làm tròn lên: ví dụ 1.5 ngày -> tính 2 ngày)
        TimeSpan thoiGianThue = request.NgayTra.Value - request.NgayNhan.Value;
        int soNgay = (int)Math.Ceiling(thoiGianThue.TotalDays);
        if (soNgay <= 0) soNgay = 1; // Đảm bảo tối thiểu tính 1 ngày

        decimal tongTien = phongs.Sum(p => p.LoaiPhong.GiaPhong) * soNgay;
        var datPhong = new DatPhong
        {
            KhachHangId = request.KhachHangId,
            NhanVienId = request.NhanVienId,
            NgayDat = DateTime.Now,
            NgayNhan = request.NgayNhan,
            NgayTra = request.NgayTra,
            TrangThai = "ChoXacNhan",
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
    [HttpGet("tra-cuu")]
    public async Task<IActionResult> TraCuuDon([FromQuery] string sdt)
    {
        if (string.IsNullOrWhiteSpace(sdt))
            return BadRequest("Vui lòng cung cấp số điện thoại.");

        // Tìm các đơn đặt phòng của khách hàng có SĐT tương ứng
        var donDatPhongs = await _context.DatPhongs
            .Include(d => d.KhachHang)
            .Include(d => d.ChiTietDatPhongs)
                .ThenInclude(ct => ct.Phong)
                    .ThenInclude(p => p.LoaiPhong)
            .Where(d => d.KhachHang != null && d.KhachHang.SoDienThoai == sdt.Trim())
            .OrderByDescending(d => d.NgayDat)
            .Select(d => new
            {
                DatPhongId = d.DatPhongId,
                MaDon = $"#DP-{d.DatPhongId:D5}",
                NgayDat = d.NgayDat,
                TrangThai = string.IsNullOrWhiteSpace(d.TrangThai) ? "Chờ xác nhận" : d.TrangThai,
                NgayNhan = d.NgayNhan,
                NgayTra = d.NgayTra,
                TongTien = d.TongTien,
                // Lấy thông tin phòng đầu tiên trong đơn (nếu có)
                Phong = d.ChiTietDatPhongs.FirstOrDefault() != null ? new
                {
                    SoPhong = d.ChiTietDatPhongs.First().Phong.SoPhong,
                    TenLoaiPhong = d.ChiTietDatPhongs.First().Phong.LoaiPhong.TenLoai
                } : null
            })
            .ToListAsync();

        return Ok(donDatPhongs);
    }
    // PUT: api/DatPhong/5/trang-thai
    // Mã trạng thái khớp CK_DatPhong_TrangThai trong SQL Server.
    // ChoXacNhan -> DaDat -> DaNhanPhong. Check-out được xử lý riêng.
    [HttpPut("{id:int}/trang-thai")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] CapNhatTrangThaiRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.TrangThai))
            return BadRequest("Vui lòng cung cấp trạng thái.");

        // Chấp nhận một vài alias cũ từ WinForms nhưng CHỈ lưu mã SQL hợp lệ.
        static string ChuanHoa(string? value) => (value ?? "").Trim().ToLowerInvariant() switch
        {
            "choxacnhan" or "chờ xác nhận" => "ChoXacNhan",
            "dadat" or "daxacnhan" => "DaDat",
            "danhanphong" or "dango" => "DaNhanPhong",
            "datraphong" or "datra" => "DaTraPhong",
            "dahuy" or "đã hủy" => "DaHuy",
            _ => ""
        };

        var mucTieu = ChuanHoa(request.TrangThai);
        if (mucTieu.Length == 0)
            return BadRequest("Trạng thái yêu cầu không nằm trong danh sách được hỗ trợ.");

        var datPhong = await _context.DatPhongs
            .Include(d => d.ChiTietDatPhongs)
                .ThenInclude(ct => ct.Phong)
            .FirstOrDefaultAsync(d => d.DatPhongId == id);

        if (datPhong == null)
            return NotFound("Không tìm thấy đơn đặt phòng.");

        var hienTai = ChuanHoa(datPhong.TrangThai);
        bool hopLe = (hienTai == "ChoXacNhan" && (mucTieu == "DaDat" || mucTieu == "DaHuy"))
                  || (hienTai == "DaDat" && (mucTieu == "DaNhanPhong" || mucTieu == "DaHuy"));
        if (!hopLe)
            return Conflict(new { Message = $"Không thể chuyển từ {hienTai} sang {mucTieu}." });

        if (mucTieu == "DaNhanPhong")
        {
            if (datPhong.ChiTietDatPhongs.Count == 0)
                return Conflict("Đơn chưa có phòng để nhận.");
            if (datPhong.ChiTietDatPhongs.Any(ct => ct.Phong == null ||
                new[] { "dangsudung", "dango", "đang có khách" }
                    .Contains((ct.Phong.TrangThai ?? "").Trim().ToLowerInvariant())))
                return Conflict("Một trong các phòng đã có khách hoặc không còn tồn tại.");
        }

        datPhong.TrangThai = mucTieu;
        if (mucTieu == "DaNhanPhong")
        {
            foreach (var ct in datPhong.ChiTietDatPhongs)
                ct.Phong.TrangThai = "DangSuDung"; // Giá trị trong SQL script CK_Phong_TrangThai.
        }
        else if (mucTieu == "DaHuy")
        {
            foreach (var ct in datPhong.ChiTietDatPhongs)
            {
                var phongId = ct.PhongId;
                var coDonKhac = await _context.ChiTietDatPhongs.AnyAsync(x =>
                    x.PhongId == phongId && x.DatPhongId != id &&
                    (x.DatPhong.TrangThai == "ChoXacNhan" ||
                     x.DatPhong.TrangThai == "DaDat" ||
                     x.DatPhong.TrangThai == "DaNhanPhong"));
                if (!coDonKhac && ct.Phong != null &&
                    !new[] { "dangsudung", "dango", "đang có khách" }
                        .Contains((ct.Phong.TrangThai ?? "").Trim().ToLowerInvariant()))
                    ct.Phong.TrangThai = "Trong";
            }
        }

        try
        {
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Cập nhật trạng thái thành công.", TrangThai = datPhong.TrangThai });
        }
        catch (DbUpdateException)
        {
            // Không trả stack trace/chi tiết SQL nội bộ cho giao diện WinForms.
            return Conflict(new { Message = "Không lưu được trạng thái. Kiểm tra ràng buộc trạng thái phòng và đơn trong SQL Server." });
        }
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
    // PUT: api/DatPhong/5/huy-don
    // Dùng chung quy tắc chuyển trạng thái; tránh ghi giá trị "Đã hủy" sai CHECK.
    // TODO: Bắt buộc xác thực và kiểm tra người sở hữu đơn ở tầng API trước khi triển khai công khai.
    [HttpPut("{id:int}/huy-don")]
    public Task<IActionResult> HuyDonKhachHangTuLam(int id)
        => UpdateStatus(id, new CapNhatTrangThaiRequest { TrangThai = "DaHuy" });
}
