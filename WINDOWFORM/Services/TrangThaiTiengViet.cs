using System;
using System.Globalization;
using System.Text;

namespace HotelManager.Services
{
    /// <summary>Chỉ chuyển mã thành nhãn giao diện. Không sửa giá trị gửi API/SQL.</summary>
    public static class TrangThaiTiengViet
    {
        private static string Key(string? text)
        {
            if (string.IsNullOrWhiteSpace(text)) return "";
            var normalized = text.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder();
            foreach (char c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark &&
                    !char.IsWhiteSpace(c) && c != '-' && c != '_')
                    builder.Append(c == 'đ' ? 'd' : c);
            }
            return builder.ToString();
        }

        public static string Phong(string? code) => Key(code) switch
        {
            "trong" or "available" => "Trống",
            "dadat" or "booked" => "Đã đặt",
            "dango" or "dangcokhach" or "dangsudung" or "occupied" => "Đang có khách",
            "dangdon" or "dangdonphong" => "Đang dọn phòng",
            "dangbaotri" or "baotri" or "maintenance" => "Đang bảo trì",
            _ => string.IsNullOrWhiteSpace(code) ? "Chưa xác định" : code.Trim()
        };

        public static string Don(string? code) => Key(code) switch
        {
            "dadat" => "Đã đặt",
            "choxacnhan" => "Chờ xác nhận",
            "daxacnhan" => "Đã xác nhận",
            "dango" or "danhanphong" => "Đã nhận phòng",
            "datra" or "datraphong" => "Đã trả phòng",
            "dathanhtoan" => "Đã thanh toán",
            "dahuy" => "Đã hủy",
            _ => string.IsNullOrWhiteSpace(code) ? "Chưa xác định" : code.Trim()
        };

        public static string VaiTro(string? code) => Key(code) switch
        {
            "admin" or "quanly" => "Quản lý",
            "staff" or "nhanvien" or "letan" => "Nhân viên / Lễ tân",
            _ => string.IsNullOrWhiteSpace(code) ? "Chưa phân quyền" : code.Trim()
        };
    }
}
