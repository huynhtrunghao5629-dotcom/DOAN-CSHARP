using System;
using System.Collections.Generic;

namespace QuanLyKhachSan.Web.Models;

// Chỉ ánh xạ các trường cần hiển thị từ GET api/DatPhong/{id}.
// Không đưa CCCD hay thông tin nhạy cảm của API vào trang xác nhận.
public class ChiTietDonViewModel
{
    public int DatPhongId { get; set; }
    public int KhachHangId { get; set; }
    public string TenKhachHang { get; set; } = string.Empty;
    public string SoDienThoai { get; set; } = string.Empty;
    public DateTime? NgayDat { get; set; }
    public DateTime? NgayNhan { get; set; }
    public DateTime? NgayTra { get; set; }
    public string TrangThai { get; set; } = string.Empty;
    public decimal TongTien { get; set; }
    public List<PhongChiTietDonViewModel> ChiTietPhong { get; set; } = new();
    public string MaDon { get; set; } = string.Empty;
}

public class PhongChiTietDonViewModel
{
    public int PhongId { get; set; }
    public string SoPhong { get; set; } = string.Empty;
    public decimal GiaPhong { get; set; }
}
