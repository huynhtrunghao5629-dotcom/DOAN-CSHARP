using System;
using System.Collections.Generic;

namespace QuanLyKhachSanAPI.Models;

public partial class NhanVien
{
    public int NhanVienId { get; set; }

    public string HoTen { get; set; } = null!;

    public string? SoDienThoai { get; set; }

    public string? Email { get; set; }

    public string? ChucVu { get; set; }

    public int? TaiKhoanId { get; set; }

    public virtual ICollection<DatPhong> DatPhongs { get; set; } = new List<DatPhong>();

    public virtual TaiKhoan? TaiKhoan { get; set; }
}
