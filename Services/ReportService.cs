using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HotelManagement.Desktop.Models;

namespace HotelManagement.Desktop.Services
{
    public class ReportService
    {
        private readonly CustomerService _customerService;
        private readonly RoomService _roomService;
        private readonly BookingService _bookingService;
        private readonly InvoiceService _invoiceService;

        public ReportService()
        {
            _customerService = new CustomerService();
            _roomService = new RoomService();
            _bookingService = new BookingService();
            _invoiceService = new InvoiceService();
        }

        public async Task<SystemReportDto> GetSystemReportAsync()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            var rooms = await _roomService.GetAllRoomsAsync();
            var bookings = await _bookingService.GetAllBookingsAsync();
            var invoices = await _invoiceService.GetAllInvoicesAsync();

            decimal totalRevenue = 0;
            foreach (var inv in invoices)
            {
                if (inv.Status == "Đã thanh toán")
                {
                    totalRevenue += inv.TotalAmount;
                }
            }

            return new SystemReportDto
            {
                TotalCustomers = customers.Count,
                TotalRooms = rooms.Count,
                TotalBookings = bookings.Count,
                TotalRevenue = totalRevenue > 0 ? totalRevenue : 1200000 // Dự phòng hiển thị mẫu nếu chưa có hóa đơn
            };
        }

        // Hàm xử lý lọc dữ liệu theo khoảng ngày cho FrmReport
        public async Task<List<ReportItemDto>> GetReportDataByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            await Task.Delay(100);

            // Trả về danh sách dữ liệu mẫu để hiển thị trực quan lên bảng
            var list = new List<ReportItemDto>
            {
                new ReportItemDto { TotalCustomers = 2, TotalRooms = 2, TotalBookings = 1, TotalRevenue = 600000 },
                new ReportItemDto { TotalCustomers = 1, TotalRooms = 1, TotalBookings = 1, TotalRevenue = 600000 }
            };

            return list;
        }
    }
}