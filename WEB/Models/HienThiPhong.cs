namespace QuanLyKhachSan.Web.Models;

// Chuyển mã lưu ở SQL thành nhãn tiếng Việt; không sửa dữ liệu nguồn.
public static class HienThiPhong
{
    public static string TenLoai(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return "Hạng phòng";
        var original = value.Trim();
        var normalized = VanBanTiengViet.BoDau(original);
        return normalized switch
        {
            "phong don" or "single" or "single room" or "standard" or "standard room" => "Phòng đơn",
            "phong doi" or "double" or "double room" or "twin" => "Phòng đôi",
            "phong vip" or "vip" or "suite" or "suite room" => "Phòng VIP",
            "deluxe" or "deluxe room" => "Phòng cao cấp",
            "family" or "family room" => "Phòng gia đình",
            _ => original
        };
    }

    public static string TrangThai(string? value) => VanBanTiengViet.BoDau(value) switch
    {
        "trong" or "available" => "Còn trống",
        "dadat" or "da dat" or "booked" => "Đã đặt",
        "dang co khach" or "dangsu dung" or "dangsudung" or "occupied" or "dango" => "Đang có khách",
        "bao tri" or "baotri" or "dang bao tri" or "dangbaotri" or "maintenance" => "Đang bảo trì",
        "dang don" or "dangdon" => "Đang dọn phòng",
        _ => "Chưa xác định"
    };

    public static bool DangBaoTri(string? value) => TrangThai(value) == "Đang bảo trì";
    public static bool DangTrong(string? value) => TrangThai(value) == "Còn trống";

}
