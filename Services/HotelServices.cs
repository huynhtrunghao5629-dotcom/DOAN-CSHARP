using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HotelManagement.Desktop.Models;

namespace HotelManagement.Desktop.Services
{
    // ValidationHelper: Xử lý kiểm tra dữ liệu đầu vào chuẩn xác[cite: 2]
    public static class ValidationHelper
    {
        public static (bool IsValid, string Message) ValidateCustomer(CustomerDto customer)
        {
            if (string.IsNullOrWhiteSpace(customer.FullName))
                return (false, "Họ tên khách hàng không được để trống!");

            if (string.IsNullOrWhiteSpace(customer.PhoneNumber) || !Regex.IsMatch(customer.PhoneNumber, @"^\d{10,11}$"))
                return (false, "Số điện thoại không hợp lệ (phải gồm 10-11 chữ số)!");

            if (!string.IsNullOrWhiteSpace(customer.Email) && !Regex.IsMatch(customer.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return (false, "Định dạng Email không hợp lệ!");

            return (true, string.Empty);
        }

        public static (bool IsValid, string Message) ValidateRoom(RoomDto room)
        {
            if (string.IsNullOrWhiteSpace(room.RoomNumber))
                return (false, "Số phòng không được để trống!");

            if (room.PricePerNight <= 0)
                return (false, "Giá phòng phải lớn hơn 0 VNĐ!");

            return (true, string.Empty);
        }
    }

    // RoomService Mockup/Client phục vụ riêng cho Desktop[cite: 2]
    public class RoomService
    {
        private readonly List<RoomDto> _rooms = new List<RoomDto>
        {
            new RoomDto { RoomId = 1, RoomNumber = "101", RoomTypeName = "Đơn Standard", PricePerNight = 300000, Status = RoomStatus.Available },
            new RoomDto { RoomId = 2, RoomNumber = "102", RoomTypeName = "Đôi VIP", PricePerNight = 600000, Status = RoomStatus.Occupied }
        };

        public async Task<List<RoomDto>> GetAllRoomsAsync(string keyword = "")
        {
            await Task.Delay(100); // Giả lập bất đồng bộ
            if (string.IsNullOrWhiteSpace(keyword)) return _rooms;

            return _rooms.FindAll(r => r.RoomNumber.Contains(keyword) || r.RoomTypeName.Contains(keyword));
        }

        public async Task<bool> SaveRoomAsync(RoomDto room)
        {
            await Task.Delay(100);
            var existing = _rooms.Find(r => r.RoomId == room.RoomId);
            if (existing == null)
            {
                room.RoomId = _rooms.Count + 1;
                _rooms.Add(room);
            }
            else
            {
                existing.RoomNumber = room.RoomNumber;
                existing.PricePerNight = room.PricePerNight;
                existing.Status = room.Status;
            }
            return true;
        }

        public async Task<bool> DeleteRoomAsync(int roomId)
        {
            await Task.Delay(100);
            var room = _rooms.Find(r => r.RoomId == roomId);
            if (room != null)
            {
                _rooms.Remove(room);
                return true;
            }
            return false;
        }
    }
}