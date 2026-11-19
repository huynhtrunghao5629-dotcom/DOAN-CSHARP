using System;

namespace HotelManagement.Desktop.Models
{
    public class BaoCaoDoanhThuViewModel
    {
        public DateTime Ngay { get; set; }
        public int SoLuotDatPhong { get; set; }
        public decimal TienPhong { get; set; }
        public decimal TienDichVu { get; set; }
        public decimal TongDoanhThu => TienPhong + TienDichVu;
    }
}