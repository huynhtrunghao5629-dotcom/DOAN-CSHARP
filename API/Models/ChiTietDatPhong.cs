using System;
using System.Collections.Generic;

namespace QuanLyKhachSanAPI.Models;

public partial class ChiTietDatPhong
{
    public int DatPhongId { get; set; }

    public int PhongId { get; set; }

    public decimal GiaPhong { get; set; }

    public virtual DatPhong DatPhong { get; set; } = null!;

    public virtual Phong Phong { get; set; } = null!;
}
