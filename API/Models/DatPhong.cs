using System;
using System.Collections.Generic;

namespace QuanLyKhachSanAPI.Models;

public partial class DatPhong
{
    public int DatPhongId { get; set; }

    public int KhachHangId { get; set; }

    public int? NhanVienId { get; set; }

    public DateTime NgayDat { get; set; }

    public DateTime? NgayNhan { get; set; }
    public DateTime? NgayTra { get; set; }

    public string TrangThai { get; set; } = null!;

    public decimal TongTien { get; set; }

    public virtual ICollection<ChiTietDatPhong> ChiTietDatPhongs { get; set; } = new List<ChiTietDatPhong>();

    public virtual KhachHang KhachHang { get; set; } = null!;

    public virtual NhanVien? NhanVien { get; set; }

    public virtual ICollection<SuDungDichVu> SuDungDichVus { get; set; } = new List<SuDungDichVu>();

    public virtual ICollection<ThanhToan> ThanhToans { get; set; } = new List<ThanhToan>();
}
