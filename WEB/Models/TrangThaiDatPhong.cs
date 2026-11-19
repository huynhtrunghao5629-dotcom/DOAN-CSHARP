namespace QuanLyKhachSan.Web.Models;

// Chỉ chuyển mã trạng thái sang nhãn hiển thị, không ghi nhãn vào SQL.
public static class TrangThaiDatPhong
{
    public static string HienThi(string? value) => (value ?? "").Trim().ToLowerInvariant() switch
    {
        "choxacnhan" or "chờ xác nhận" => "Chờ xác nhận",
        "dadat" or "đã đặt" or "daxacnhan" => "Đã đặt (đã xác nhận)",
        "danhanphong" or "dango" => "Đã nhận phòng",
        "datraphong" or "datra" => "Đã trả phòng",
        "dahuy" or "đã hủy" => "Đã hủy",
        "dathanhtoan" => "Đã thanh toán",
        _ => string.IsNullOrWhiteSpace(value) ? "Chưa xác định" : "Trạng thái khác"
    };

    public static string MauBadge(string? value) => (value ?? "").Trim().ToLowerInvariant() switch
    {
        "choxacnhan" => "bg-warning text-dark",
        "dadat" or "daxacnhan" => "bg-success",
        "danhanphong" or "dango" => "bg-primary",
        "datraphong" or "datra" or "dathanhtoan" => "bg-secondary",
        "dahuy" => "bg-danger",
        _ => "bg-secondary"
    };

    public static bool ChoPhepHuy(string? value) =>
        string.Equals(value?.Trim(), "ChoXacNhan", StringComparison.OrdinalIgnoreCase);

    public static bool DaHoanThanh(string? value) =>
        new[] { "datraphong", "datra", "dathanhtoan" }.Contains((value ?? "").Trim().ToLowerInvariant());

    public static bool ConHoatDong(string? value) =>
        new[] { "choxacnhan", "dadat", "danhanphong", "daxacnhan", "dango" }
            .Contains((value ?? "").Trim().ToLowerInvariant());
}
