namespace QuanLyKhachSan.Web.Models;

// Endpoint GET api/LoaiPhong (không cần thay đổi API).
public sealed class LoaiPhongViewModel
{
    public int LoaiPhongId { get; set; }
    public string TenLoai { get; set; } = string.Empty;
    public int SoNguoiToiDa { get; set; }
    public decimal GiaPhong { get; set; }
    public string? MoTa { get; set; }
}
