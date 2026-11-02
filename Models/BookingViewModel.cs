using System;

namespace HotelManagement.Desktop.Models
{
    public class BookingViewModel
    {
        public int Id { get; set; }
        public string TenKhachHang { get; set; } = string.Empty;
        public string SoDienThoai { get; set; } = string.Empty;
        public string TenLoaiPhong { get; set; } = string.Empty;
        public string? SoPhong { get; set; }
        public DateTime NgayNhanPhong { get; set; }
        public DateTime NgayTraPhong { get; set; }
        public decimal TongTien { get; set; }
        public string TrangThai { get; set; } = "ChoXacNhan"; // ChoXacNhan, DaXacNhan, DaNhanPhong, DaThanhToan, DaHuy
        public string GhiChu { get; set; } = string.Empty;
    }
}