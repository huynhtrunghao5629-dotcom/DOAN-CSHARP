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
        public int RoomId { get; set; }
        public string? RoomNumber { get; set; }
        public int RoomTypeId { get; set; }
        public string? RoomTypeName { get; set; }
        public decimal PricePerNight { get; set; }
        public RoomStatus Status { get; set; }
    }

    // API JSON: khachHangId, hoTen, cccd, soDienThoai, email.
    // Keep the public properties used by existing WinForms code.
    public class CustomerDto
    {
        [JsonPropertyName("khachHangId")]
        public int CustomerId { get; set; }

        [JsonPropertyName("hoTen")]
        public string? FullName { get; set; }

        [JsonPropertyName("cccd")]
        public string? IdentityCard { get; set; }

        [JsonPropertyName("soDienThoai")]
        public string? PhoneNumber { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("diaChi")]
        public string? Address { get; set; }
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
}