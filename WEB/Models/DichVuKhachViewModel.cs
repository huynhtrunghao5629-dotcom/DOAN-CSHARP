namespace QuanLyKhachSan.Web.Models;

// Chỉ phục vụ hiển thị dịch vụ phía Web, không dùng model Desktop.
public sealed class DichVuKhachViewModel
{
    public int DichVuId { get; set; }
    public string TenDichVu { get; set; } = string.Empty;
    public decimal DonGia { get; set; }
    public string? DonViTinh { get; set; }
    public string? MoTa { get; set; }
}

public static class HienThiDichVu
{
    // SQL hiện có một số dữ liệu không dấu: chuẩn hóa khi hiển thị mà không sửa DB/API.
    public static string Ten(string? name) => VanBanTiengViet.BoDau(name?.Trim()) switch
    {
        "an sang" => "Ăn sáng",
        "giat ui" => "Giặt ủi",
        "nuoc suoi" => "Nước suối",
        "thue xe" => "Thuê xe",
        _ => string.IsNullOrWhiteSpace(name) ? "Dịch vụ khách sạn" : name.Trim()
    };

    public static string MoTa(string? value) => VanBanTiengViet.BoDau(value?.Trim()) switch
    {
        "suat an sang" => "Suất ăn sáng",
        "dich vu giat ui" => "Dịch vụ giặt ủi",
        "nuoc suoi dong chai" => "Nước suối đóng chai",
        "thue xe trong ngay" => "Thuê xe trong ngày",
        _ => value?.Trim() ?? string.Empty
    };

    public static string DonVi(string? value) => VanBanTiengViet.BoDau(value?.Trim()) switch
    {
        "suat" => "suất",
        "lan" => "lần",
        "chai" => "chai",
        "ngay" => "ngày",
        "nguoi" => "người",
        "bo" => "bộ",
        "phong" => "phòng",
        "dem" => "đêm",
        "gio" => "giờ",
        "kg" => "kg",
        "" => "lượt",
        _ => value!.Trim()
    };

    public static string Icon(string? name) => VanBanTiengViet.BoDau(name?.Trim()) switch
    {
        "an sang" => "bi-cup-hot",
        "giat ui" => "bi-basket",
        "nuoc suoi" => "bi-droplet",
        "thue xe" => "bi-car-front",
        _ => "bi-stars"
    };
}
