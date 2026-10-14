using System;

namespace HotelManagement.Desktop.Models
{
    public class BookingDto
    {
        public int BookingId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } // Dùng để hiển thị tên khách hàng lên lưới
        public int RoomId { get; set; }
        public string RoomNumber { get; set; }   // Dùng để hiển thị số phòng lên lưới
        public DateTime CheckInDate { get; set; } = DateTime.Now;
        public DateTime CheckOutDate { get; set; } = DateTime.Now.AddDays(1);
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = "Đang đặt"; // Trạng thái: Đang đặt, Đã nhận phòng, Đã trả phòng, Đã hủy
    }
}