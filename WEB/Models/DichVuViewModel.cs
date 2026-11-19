using System;

namespace HotelManagement.Desktop.Models
{
    public class DichVuViewModel
    {
        public int Id { get; set; }
        public string TenDichVu { get; set; } = string.Empty;
        public decimal DonGia { get; set; }
        public string DonViTinh { get; set; } = string.Empty;
    }

    public class ChiTietDichVuViewModel
    {
        public int Id { get; set; }
        public int DonDatPhongId { get; set; }
        public string PhongId { get; set; } = string.Empty;
        public string SoPhong { get; set; } = string.Empty;
        public int DichVuId { get; set; }
        public string TenDichVu { get; set; } = string.Empty;
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => SoLuong * DonGia;
        public DateTime ThoiGian { get; set; } = DateTime.Now;
    }
}