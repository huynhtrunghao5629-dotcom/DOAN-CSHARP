namespace HotelManagement.Desktop.Models
{
    public class PhongViewModel
    {
        public string PhongId { get; set; } = string.Empty;
        public string SoPhong { get; set; } = string.Empty;
        public string TrangThai { get; set; } = "Trong";
        public string TenLoaiPhong { get; set; } = string.Empty;
        public decimal GiaPhong { get; set; }
    }
}