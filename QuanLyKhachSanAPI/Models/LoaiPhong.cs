using System;
using System.Collections.Generic;

namespace QuanLyKhachSanAPI.Models;

public partial class LoaiPhong
{
    public int LoaiPhongId { get; set; }

    public string TenLoai { get; set; } = null!;

    public int SoNguoiToiDa { get; set; }

    public decimal GiaPhong { get; set; }

    public string? MoTa { get; set; }

    public virtual ICollection<Phong> Phongs { get; set; } = new List<Phong>();
}
