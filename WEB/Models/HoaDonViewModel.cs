using System;
using System.Collections.Generic;

namespace HotelManagement.Desktop.Models
{
    public class HoaDonViewModel
    {
        public int Id { get; set; }
        public int DonDatPhongId { get; set; }
        public string TenKhachHang { get; set; } = string.Empty;
        public string SoPhong { get; set; } = string.Empty;
        public DateTime NgayNhanPhong { get; set; }
        public DateTime NgayTraPhong { get; set; } = DateTime.Now;
        public int SoNgayO => Math.Max(1, (NgayTraPhong.Date - NgayNhanPhong.Date).Days);
        public decimal GiaPhong { get; set; }
        public decimal TienPhong => SoNgayO * GiaPhong;
        public decimal TienDichVu { get; set; }
        public decimal TongTien => TienPhong + TienDichVu;
        public string TrangThai { get; set; } = "ChuaThanhToan"; // ChuaThanhToan, DaThanhToan
        public DateTime NgayThanhToan { get; set; } = DateTime.Now;
        public string NguoiLap { get; set; } = string.Empty;
        public List<ChiTietDichVuViewModel> DanhSachDichVu { get; set; } = new List<ChiTietDichVuViewModel>();
    }
}