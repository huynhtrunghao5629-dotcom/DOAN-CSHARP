using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using QuanLyKhachSanAPI.Models;

namespace QuanLyKhachSanAPI.Controllers;

public class TaoDatPhongRequest
{
    public int KhachHangId { get; set; }
    public int? NhanVienId { get; set; }
    public DateTime? NgayNhan { get; set; }
    public DateTime? NgayTra { get; set; }
    public int? SoKhach { get; set; }
    public List<int> DanhSachPhongId { get; set; } = new();
}

public sealed class HuyDonKhachHangRequest
{
    public int KhachHangId { get; set; }
    public string SoDienThoai { get; set; } = string.Empty;
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

    // POST: api/DatPhong (cả Web và WinForms sử dụng cùng hợp đồng)
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TaoDatPhongRequest request)
    {
        if (request.NgayNhan == null || request.NgayTra == null)
            return BadRequest("Thiếu ngày nhận hoặc ngày trả phòng.");

        // SQL lưu DATE nên chuẩn hóa về ngày để tránh tính tiền lệch do phần giờ.
        var ngayNhan = request.NgayNhan.Value.Date;
        var ngayTra = request.NgayTra.Value.Date;

        if (ngayNhan < DateTime.Today || ngayTra <= ngayNhan)
            return BadRequest("Ngày nhận/trả không hợp lệ.");

        if (request.SoKhach.HasValue && request.SoKhach.Value <= 0)
            return BadRequest("Số khách phải lớn hơn 0.");

        var ids = request.DanhSachPhongId?.Distinct().ToList() ?? new List<int>();
        if (ids.Count == 0 || ids.Any(id => id <= 0) ||
            ids.Count != (request.DanhSachPhongId?.Count ?? 0))
            return BadRequest("Danh sách phòng không hợp lệ hoặc có phòng trùng.");

        // SERIALIZABLE: các yêu cầu đặt trùng lịch không thể cùng vượt qua kiểm tra.
        await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        try
        {
            if (!await _context.KhachHangs.AnyAsync(k => k.KhachHangId == request.KhachHangId))
                return BadRequest("Khách hàng không tồn tại.");

            if (request.NhanVienId.HasValue &&
                !await _context.NhanViens.AnyAsync(n => n.NhanVienId == request.NhanVienId.Value))
                return BadRequest("Nhân viên không tồn tại.");

            var phongs = await _context.Phongs
                .Include(p => p.LoaiPhong)
                .Where(p => ids.Contains(p.PhongId))
                .ToListAsync();

            if (phongs.Count != ids.Count)
                return BadRequest("Một số phòng không tồn tại.");

            if (phongs.Any(p => p.TrangThai == "DangBaoTri" || p.LoaiPhong == null))
                return Conflict("Một trong các phòng đang bảo trì hoặc không có thông tin hạng phòng.");

            // Web gửi SoKhach; WinForms cũ có thể không gửi nên trường này là nullable.
            // Với đơn nhiều phòng, sức chứa là tổng sức chứa của các phòng.
            if (request.SoKhach.HasValue)
            {
                var sucChua = phongs.Sum(p => Math.Max(0, p.LoaiPhong.SoNguoiToiDa));
                if (sucChua <= 0)
                    return Conflict("Hạng phòng chưa có thông tin sức chứa hợp lệ.");
                if (request.SoKhach.Value > sucChua)
                    return BadRequest($"Số khách vượt quá sức chứa tối đa của phòng đã chọn ({sucChua} người).");
            }

            var lichTrung = await _context.ChiTietDatPhongs
                            .Include(ct => ct.Phong)
                            .Include(ct => ct.DatPhong)
                            .Where(ct =>
                                ids.Contains(ct.PhongId) &&
                                ct.DatPhong.TrangThai != "DaHuy" &&
                                ct.DatPhong.TrangThai != "DaTraPhong" &&
                                ct.DatPhong.NgayNhan < ngayTra &&
                                ct.DatPhong.NgayTra > ngayNhan)
                            .Select(ct => new
                            {
                                ct.Phong.SoPhong,
                                ct.DatPhong.NgayNhan,
                                ct.DatPhong.NgayTra
                            })
                            .FirstOrDefaultAsync();

            if (lichTrung != null)
            {
                return Conflict(
                    $"Phòng {lichTrung.SoPhong} đã có lịch đặt từ {lichTrung.NgayNhan:dd/MM/yyyy} đến {lichTrung.NgayTra:dd/MM/yyyy}. Vui lòng chọn khoảng ngày khác.");
            }

            var soNgay = (ngayTra - ngayNhan).Days;
            var don = new DatPhong
            {
                KhachHangId = request.KhachHangId,
                NhanVienId = request.NhanVienId,
                NgayDat = DateTime.Now,
                NgayNhan = ngayNhan,
                NgayTra = ngayTra,
                TrangThai = "ChoXacNhan",
                TongTien = phongs.Sum(p => p.LoaiPhong.GiaPhong) * soNgay
            };

            foreach (var phong in phongs)
            {
                don.ChiTietDatPhongs.Add(new ChiTietDatPhong
                {
                    PhongId = phong.PhongId,
                    GiaPhong = phong.LoaiPhong.GiaPhong
                });

            }

            _context.DatPhongs.Add(don);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return CreatedAtAction(nameof(GetById), new { id = don.DatPhongId }, new
            {
                don.DatPhongId,
                don.TongTien,
                don.TrangThai,
                ThongBao = "Đặt phòng thành công, chờ xác nhận."
            });
        }
        catch (DbUpdateException)
        {
            return Conflict("Không thể tạo đơn do dữ liệu hoặc lịch phòng đã thay đổi.");
        }
        catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 1205)
        {
            return Conflict("Có đơn khác đang được tạo đồng thời. Vui lòng tải lại phòng trống.");
        }
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
    [Authorize(Roles = "Admin,NhanVien")]
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
                ct.Phong.TrangThai = "DangSuDung";
        }
        else if (mucTieu == "DaHuy")
        {
            foreach (var ct in datPhong.ChiTietDatPhongs)
            {
                if (ct.Phong != null &&
                    string.Equals(ct.Phong.TrangThai, "DaDat", StringComparison.OrdinalIgnoreCase))
                {
                    ct.Phong.TrangThai = "Trong";
                }
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
    [Authorize(Roles = "Admin,NhanVien")]
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
    [Authorize(Roles = "Admin,NhanVien")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var dp = await _context.DatPhongs
            .Include(d => d.ChiTietDatPhongs)
            .Include(d => d.SuDungDichVus)
            .Include(d => d.ThanhToans)
            .FirstOrDefaultAsync(d => d.DatPhongId == id);

        if (dp == null) return NotFound("Không tìm thấy đơn đặt phòng.");

        // Bảo toàn lịch sử tài chính: chỉ cho xóa đơn chưa phát sinh dịch vụ/thanh toán.
        if (dp.SuDungDichVus.Any() || dp.ThanhToans.Any() ||
            dp.TrangThai == "DaNhanPhong" || dp.TrangThai == "DaTraPhong")
            return Conflict("Đơn đang/đã sử dụng hoặc có giao dịch, không được xóa cứng.");
        foreach (var ct in dp.ChiTietDatPhongs)
        {
            var coDonKhac = await _context.ChiTietDatPhongs.AnyAsync(x =>
                x.PhongId == ct.PhongId && x.DatPhongId != id &&
                x.DatPhong.TrangThai != "DaHuy" &&
                x.DatPhong.TrangThai != "DaTraPhong");
            var phong = await _context.Phongs.FindAsync(ct.PhongId);
            if (!coDonKhac && phong != null && phong.TrangThai != "DangSuDung")
                phong.TrangThai = "Trong";
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
    // Khách chỉ được tự hủy đơn của chính mình và chỉ khi còn ChoXacNhan.
    [HttpPut("{id:int}/huy-don")]
    public async Task<IActionResult> HuyDonKhachHangTuLam(
        int id,
        [FromBody] HuyDonKhachHangRequest request)
    {
        if (id <= 0 || request == null || request.KhachHangId <= 0 ||
            string.IsNullOrWhiteSpace(request.SoDienThoai))
            return BadRequest("Thông tin hủy đơn không hợp lệ.");

        var soDienThoai = request.SoDienThoai.Trim();

        await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        try
        {
            var datPhong = await _context.DatPhongs
                .Include(d => d.KhachHang)
                .Include(d => d.ChiTietDatPhongs)
                    .ThenInclude(ct => ct.Phong)
                .FirstOrDefaultAsync(d => d.DatPhongId == id);

            if (datPhong == null)
                return NotFound("Không tìm thấy đơn đặt phòng.");

            // Không tiết lộ đơn của người khác.
            if (datPhong.KhachHangId != request.KhachHangId ||
                datPhong.KhachHang == null ||
                !string.Equals(datPhong.KhachHang.SoDienThoai?.Trim(), soDienThoai, StringComparison.Ordinal))
                return NotFound("Không tìm thấy đơn đặt phòng thuộc tài khoản này.");

            if (!string.Equals(datPhong.TrangThai, "ChoXacNhan", StringComparison.Ordinal))
                return Conflict("Chỉ đơn đang chờ xác nhận mới được khách hàng tự hủy.");

            datPhong.TrangThai = "DaHuy";

            foreach (var ct in datPhong.ChiTietDatPhongs)
            {
                if (ct.Phong != null &&
                    string.Equals(ct.Phong.TrangThai, "DaDat", StringComparison.OrdinalIgnoreCase))
                {
                    ct.Phong.TrangThai = "Trong";
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new
            {
                Message = "Đã hủy yêu cầu đặt phòng.",
                datPhong.DatPhongId,
                datPhong.TrangThai
            });
        }
        catch (DbUpdateException)
        {
            return Conflict("Không thể hủy đơn do trạng thái dữ liệu vừa thay đổi. Vui lòng tải lại.");
        }
        catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 1205)
        {
            return Conflict("Đơn đang được xử lý đồng thời. Vui lòng tải lại trạng thái đơn.");
        }
    }
}
