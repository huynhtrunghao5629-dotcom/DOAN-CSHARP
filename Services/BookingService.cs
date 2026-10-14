using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelManagement.Desktop.Models;

namespace HotelManagement.Desktop.Services
{
    public class BookingService
    {
        // Sử dụng từ khóa static để giữ dữ liệu dùng chung cho toàn bộ ứng dụng
        private static readonly List<BookingDto> _bookings = new List<BookingDto>
        {
            new BookingDto
            {
                BookingId = 1,
                CustomerName = "Nguyễn Văn An",
                RoomNumber = "101",
                TotalPrice = 600000
            }
        };

        public async Task<List<BookingDto>> GetAllBookingsAsync()
        {
            await Task.Delay(100);
            return _bookings.ToList();
        }

        public async Task SaveBookingAsync(BookingDto booking)
        {
            await Task.Delay(100);
            if (booking.BookingId == 0)
            {
                // Tự động tăng ID tiếp theo
                booking.BookingId = _bookings.Count > 0 ? _bookings.Max(b => b.BookingId) + 1 : 1;
                _bookings.Add(booking);
            }
            else
            {
                var existing = _bookings.FirstOrDefault(b => b.BookingId == booking.BookingId);
                if (existing != null)
                {
                    existing.CustomerName = booking.CustomerName;
                    existing.RoomNumber = booking.RoomNumber;
                    existing.TotalPrice = booking.TotalPrice;
                }
            }
        }

        public async Task DeleteBookingAsync(int bookingId)
        {
            await Task.Delay(100);
            var booking = _bookings.FirstOrDefault(b => b.BookingId == bookingId);
            if (booking != null)
            {
                _bookings.Remove(booking);
            }
        }
    }
}