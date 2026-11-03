using System;
using System.Collections.Generic;

namespace QuanLyKhachSanAPI.Models;

public partial class KhachHang
{
    public int KhachHangId { get; set; }

    public string HoTen { get; set; } = null!;

    public string? Cccd { get; set; }

    public string? SoDienThoai { get; set; }

    public string? Email { get; set; }

    public string? DiaChi { get; set; }
    public string? MatKhau { get; set; }
    public virtual ICollection<DatPhong> DatPhongs { get; set; } = new List<DatPhong>();
}
