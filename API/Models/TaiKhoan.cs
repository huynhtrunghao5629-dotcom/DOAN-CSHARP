using System;
using System.Collections.Generic;

namespace QuanLyKhachSanAPI.Models;

public partial class TaiKhoan
{
    public int TaiKhoanId { get; set; }

    public string TenDangNhap { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public string VaiTro { get; set; } = null!;

    public virtual NhanVien? NhanVien { get; set; }
}
