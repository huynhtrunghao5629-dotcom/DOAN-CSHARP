using System;
using System.Collections.Generic;

namespace QuanLyKhachSanAPI.Models;

public partial class Phong
{
    public int PhongId { get; set; }

    public string SoPhong { get; set; } = null!;

    public int LoaiPhongId { get; set; }

    public string TrangThai { get; set; } = null!;
    public string? HinhAnh { get; set; }

    public virtual ICollection<ChiTietDatPhong> ChiTietDatPhongs { get; set; } = new List<ChiTietDatPhong>();

    public virtual LoaiPhong LoaiPhong { get; set; } = null!;
}
