using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using HotelManagement.Desktop.Models;

namespace HotelManagement.Desktop.Services
{
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
    public class RoomService
    {
        private readonly HttpClient _client;

        // Lưu thông báo lỗi từ API để form có thể hiển thị nguyên nhân cụ thể.
        public string LastError { get; private set; } = string.Empty;

        public RoomService()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7203/")
            };
        }
        public class RoomTypeService
        {
            private readonly HttpClient _client;
            public string LastError { get; private set; } = string.Empty;

            public RoomTypeService()
            {
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };
                _client = new HttpClient(handler)
                {
                    BaseAddress = new Uri("https://localhost:7203/")
                };
            }

            public async Task<List<RoomTypeDto>> GetAllRoomTypesAsync()
            {
                LastError = string.Empty;
                try
                {
                    return await _client.GetFromJsonAsync<List<RoomTypeDto>>("api/LoaiPhong") ?? new List<RoomTypeDto>();
                }
                catch (Exception ex)
                {
                    LastError = "Không thể tải danh sách loại phòng: " + ex.Message;
                    return new List<RoomTypeDto>();
                }
            }

            public async Task<bool> CreateRoomTypeAsync(RoomTypeDto roomType)
            {
                LastError = string.Empty;
                try
                {
                    var payload = new
                    {
                        tenLoai = roomType.RoomTypeName?.Trim(),
                        giaPhong = roomType.PricePerNight
                    };

                    using var response = await _client.PostAsJsonAsync("api/LoaiPhong", payload);
                    if (response.IsSuccessStatusCode) return true;

                    LastError = await response.Content.ReadAsStringAsync();
                    return false;
                }
                catch (Exception ex)
                {
                    LastError = "Lỗi thêm loại phòng: " + ex.Message;
                    return false;
                }
            }

            public async Task<bool> UpdateRoomTypeAsync(RoomTypeDto roomType)
            {
                LastError = string.Empty;
                try
                {
                    var payload = new
                    {
                        loaiPhongId = roomType.RoomTypeId,
                        tenLoai = roomType.RoomTypeName?.Trim(),
                        giaPhong = roomType.PricePerNight
                    };

                    using var response = await _client.PutAsJsonAsync($"api/LoaiPhong/{roomType.RoomTypeId}", payload);
                    if (response.IsSuccessStatusCode) return true;

                    LastError = await response.Content.ReadAsStringAsync();
                    return false;
                }
                catch (Exception ex)
                {
                    LastError = "Lỗi cập nhật loại phòng: " + ex.Message;
                    return false;
                }
            }

            public async Task<bool> DeleteRoomTypeAsync(int roomTypeId)
            {
                LastError = string.Empty;
                try
                {
                    using var response = await _client.DeleteAsync($"api/LoaiPhong/{roomTypeId}");
                    if (response.IsSuccessStatusCode) return true;

                    LastError = await response.Content.ReadAsStringAsync();
                    return false;
                }
                catch (Exception ex)
                {
                    LastError = "Lỗi xóa loại phòng: " + ex.Message;
                    return false;
                }
            }
        }
        public async Task<List<RoomDto>> GetAllRoomsAsync(string keyword = "")
        {
            LastError = string.Empty;
            try
            {
                var rooms = await _client.GetFromJsonAsync<List<RoomDto>>("api/Phong")
                            ?? new List<RoomDto>();

                // API dùng giá trị không dấu; giao diện hiển thị tiếng Việt có dấu.
                foreach (var room in rooms)
                    room.Status = ToDisplayStatus(room.Status);

                if (string.IsNullOrWhiteSpace(keyword))
                    return rooms;

                keyword = keyword.Trim();
                return rooms.Where(r =>
                        (!string.IsNullOrWhiteSpace(r.RoomNumber) &&
                         r.RoomNumber.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrWhiteSpace(r.RoomTypeName) &&
                         r.RoomTypeName.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }
            catch (Exception ex)
            {
                LastError = "Không thể tải danh sách phòng từ API: " + ex.Message;
                return new List<RoomDto>();
            }
        }

        public async Task<bool> CreateRoomAsync(RoomDto room)
        {
            LastError = string.Empty;
            try
            {
                // Giá phòng lấy từ LoaiPhong.GiaPhong ở API, không phải từ bảng Phong.
                var payload = new
                {
                    soPhong = room.RoomNumber?.Trim(),
                    loaiPhongId = room.RoomTypeId,
                    trangThai = ToApiStatus(room.Status)
                };

                using var response = await _client.PostAsJsonAsync("api/Phong", payload);
                if (response.IsSuccessStatusCode)
                    return true;

                LastError = await ReadApiErrorAsync(response);
                return false;
            }
            catch (Exception ex)
            {
                LastError = "Không thể thêm phòng: " + ex.Message;
                return false;
            }
        }

        public async Task<bool> UpdateRoomAsync(RoomDto room)
        {
            LastError = string.Empty;
            if (room.RoomId <= 0)
            {
                LastError = "Mã phòng không hợp lệ. Vui lòng chọn phòng cần cập nhật trong bảng.";
                return false;
            }

            try
            {
                var payload = new
                {
                    phongId = room.RoomId,
                    soPhong = room.RoomNumber?.Trim(),
                    loaiPhongId = room.RoomTypeId,
                    trangThai = ToApiStatus(room.Status)
                };

                using var response = await _client.PutAsJsonAsync($"api/Phong/{room.RoomId}", payload);
                if (response.IsSuccessStatusCode)
                    return true;

                LastError = await ReadApiErrorAsync(response);
                return false;
            }
            catch (Exception ex)
            {
                LastError = "Không thể cập nhật phòng: " + ex.Message;
                return false;
            }
        }

        public async Task<bool> DeleteRoomAsync(int roomId)
        {
            LastError = string.Empty;
            if (roomId <= 0)
            {
                LastError = "Mã phòng không hợp lệ. Vui lòng chọn phòng cần xóa.";
                return false;
            }

            try
            {
                using var response = await _client.DeleteAsync($"api/Phong/{roomId}");
                if (response.IsSuccessStatusCode)
                    return true;

                LastError = await ReadApiErrorAsync(response);
                return false;
            }
            catch (Exception ex)
            {
                LastError = "Không thể xóa phòng: " + ex.Message;
                return false;
            }
        }

        private static string ToApiStatus(string? status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return "Trong";

            return status.Trim() switch
            {
                "Available" or "Trống" or "Trong" => "Trong",
                "Occupied" or "Đang có khách" or "Dang co khach" => "Dang co khach",
                "Maintenance" or "Bảo trì" or "Bao tri" => "Bao tri",
                _ => status.Trim()
            };
        }

        private static string ToDisplayStatus(string? status)
        {
            return status?.Trim() switch
            {
                "Available" or "Trống" or "Trong" => "Trống",
                "Occupied" or "Đang có khách" or "Dang co khach" => "Đang có khách",
                "Maintenance" or "Bảo trì" or "Bao tri" => "Bảo trì",
                _ => status ?? string.Empty
            };
        }

        private static async Task<string> ReadApiErrorAsync(HttpResponseMessage response)
        {
            var body = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(body))
                return $"API trả về {(int)response.StatusCode} ({response.ReasonPhrase}): {body}";

            return $"API trả về {(int)response.StatusCode} ({response.ReasonPhrase}).";
        }

        public async Task UpdateRoomStatusAsync(string roomNumber, string status)
        {
            // Chưa triển khai vì API hiện cập nhật trạng thái qua PUT api/Phong/{id}.
            await Task.CompletedTask;
        }
    }
}
