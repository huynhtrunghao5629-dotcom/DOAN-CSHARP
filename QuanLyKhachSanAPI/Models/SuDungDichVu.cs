using System;
using System.Collections.Generic;

namespace QuanLyKhachSanAPI.Models;

public partial class SuDungDichVu
{
    public int SuDungDichVuId { get; set; }

    public int DatPhongId { get; set; }

    public int DichVuId { get; set; }

    public int SoLuong { get; set; }

    public decimal DonGia { get; set; }

    public DateTime NgaySuDung { get; set; }

    public virtual DatPhong DatPhong { get; set; } = null!;

    public virtual DichVu DichVu { get; set; } = null!;
}
