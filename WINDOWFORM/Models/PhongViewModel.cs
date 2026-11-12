namespace HotelManagement.Desktop.Models
{
    public class PhongViewModel
    {
        public int PhongId { get; set; }
        public string SoPhong { get; set; } = string.Empty;
        public int LoaiPhongId { get; set; }
        public string TrangThai { get; set; } = "Trong";
        public string TenLoaiPhong { get; set; } = string.Empty;
        public decimal GiaPhong { get; set; }
    }
}
