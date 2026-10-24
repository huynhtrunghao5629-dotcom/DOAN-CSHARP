namespace QuanLyKhachSan.Web.Models
{
    public class PhongTrongDon
    {
        public string SoPhong { get; set; } = string.Empty;
        public string TenLoaiPhong { get; set; } = string.Empty;
    }

    public class LichSuDatPhongViewModel
    {
        public int DatPhongId { get; set; }
        public string MaDon { get; set; } = string.Empty;
        public DateTime? NgayDat { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public DateTime? NgayNhan { get; set; }
        public DateTime? NgayTra { get; set; }
        public decimal TongTien { get; set; }
        public PhongTrongDon? Phong { get; set; }
    }
}