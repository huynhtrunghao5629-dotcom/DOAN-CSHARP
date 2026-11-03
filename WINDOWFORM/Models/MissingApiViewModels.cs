using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace HotelManagement.Desktop.Models
{
    // DTO tương thích cấu trúc GET api/DatPhong hiện tại.
    public class BookingViewModel
    {
        [JsonPropertyName("datPhongId")]
        public int Id { get; set; }
        public string TenKhachHang { get; set; } = string.Empty;
        public string TrangThai { get; set; } = string.Empty;
        public DateTime? NgayNhan { get; set; }
        public DateTime? NgayTra { get; set; }
        public DateTime NgayNhanPhong => NgayNhan ?? DateTime.MinValue;
        public decimal TongTien { get; set; }
        public List<BookingRoomItem> DanhSachPhong { get; set; } = new();
        public string SoPhong => string.Join(", ", DanhSachPhong.Select(x => x.SoPhong));
    }

    public class BookingRoomItem
    {
        public int PhongId { get; set; }
        public string SoPhong { get; set; } = string.Empty;
        public decimal GiaPhong { get; set; }
    }

    public class ChiTietDichVuViewModel
    {
        [JsonPropertyName("suDungDichVuId")]
        public int Id { get; set; }
        public int DatPhongId { get; set; }
        public int DichVuId { get; set; }
        public string TenDichVu { get; set; } = string.Empty;
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => SoLuong * DonGia;
    }

    public class DichVuViewModel
    {
        [JsonPropertyName("dichVuId")]
        public int Id { get; set; }
        public string TenDichVu { get; set; } = string.Empty;
        public decimal DonGia { get; set; }
    }

    public class UserAccountViewModel
    {
        [JsonPropertyName("taiKhoanId")]
        public int Id { get; set; }
        [JsonPropertyName("tenDangNhap")]
        public string Username { get; set; } = string.Empty;
        [JsonIgnore]
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        [JsonPropertyName("vaiTro")]
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }

    public class BaoCaoDoanhThuViewModel
    {
        public DateTime Ngay { get; set; }
        public int SoLuotDatPhong { get; set; }
        public decimal TongDoanhThu { get; set; }
    }
}
