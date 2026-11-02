using System;

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

    public class CustomerDto
    {
        public int CustomerId { get; set; }
        public string? FullName { get; set; }
        public string? IdentityCard { get; set; } // CCCD/CMND
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
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