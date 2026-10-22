using System;
using System.Text.Json.Serialization;
namespace HotelManagement.Desktop.Models
{
    public enum RoomStatus
    {
        Available = 0, // Trống
        Occupied = 1,  // Đang có khách
        Maintenance = 2 // Bảo trì
    }

    public class RoomDto
    {
        [JsonPropertyName("phongId")]
        public int RoomId { get; set; }

        [JsonPropertyName("soPhong")]
        public string RoomNumber { get; set; }

        [JsonPropertyName("giaPhong")]
        public decimal PricePerNight { get; set; }

        [JsonPropertyName("trangThai")]
        public string Status { get; set; }

        [JsonPropertyName("TenLoaiPhong")]
        public string RoomTypeName { get; set; }

        [JsonPropertyName("loaiPhongId")]
        public int RoomTypeId { get; set; }
    }

    public class LoginRequest
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }

    public class UserSession
    {
        public string? Token { get; set; }
        public string? Username { get; set; }
        public string? Role { get; set; } // Admin, Staff
    }
    public class RoomTypeDto
    {
        [JsonPropertyName("loaiPhongId")]
        public int RoomTypeId { get; set; }

        [JsonPropertyName("tenLoai")]
        public string RoomTypeName { get; set; }

        // Giá được lấy từ loại phòng; API Phong không lưu giá riêng từng phòng.
        [JsonPropertyName("giaPhong")]
        public decimal PricePerNight { get; set; }
    }
}