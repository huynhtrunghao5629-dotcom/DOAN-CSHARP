using System;

namespace HotelManagement.Desktop.Models
{
    public class InvoiceDto
    {
        public int InvoiceId { get; set; }
        public int BookingId { get; set; }
        public string CustomerName { get; set; }
        public string RoomNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        public string PaymentMethod { get; set; } = "Tiền mặt"; // Tiền mặt, Chuyển khoản, Thẻ
        public string Status { get; set; } = "Đã thanh toán"; // Đã thanh toán, Chưa thanh toán
    }
}