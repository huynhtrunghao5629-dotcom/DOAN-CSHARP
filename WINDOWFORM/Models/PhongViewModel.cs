namespace HotelManagement.Desktop.Models
{
    public class PhongViewModel
    {
        public int PhongId { get; set; }
        public string SoPhong { get; set; } = string.Empty;
        public string TrangThai { get; set; } = "Trong";
        public string TenLoaiPhong { get; set; } = string.Empty;
        public decimal GiaPhong { get; set; }
    }
}