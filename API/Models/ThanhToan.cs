using System;
using System.Collections.Generic;

namespace QuanLyKhachSanAPI.Models;

public partial class ThanhToan
{
    public int ThanhToanId { get; set; }

    public int DatPhongId { get; set; }

    public DateTime NgayThanhToan { get; set; }

    public decimal SoTien { get; set; }

    public string PhuongThuc { get; set; } = null!;

    public string TrangThai { get; set; } = null!;

    public virtual DatPhong DatPhong { get; set; } = null!;
}
