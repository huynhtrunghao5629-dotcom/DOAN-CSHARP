namespace QuanLyKhachSan.Web.Models
{
    public class PhongViewModel
    {
        public int PhongId { get; set; }
        public string SoPhong { get; set; } = string.Empty;
        public string TrangThai { get; set; } = string.Empty;
        public int LoaiPhongId { get; set; }
        public string TenLoaiPhong { get; set; } = string.Empty;
        public decimal GiaPhong { get; set; }
        public string? HinhAnh { get; set; }
    }
}
