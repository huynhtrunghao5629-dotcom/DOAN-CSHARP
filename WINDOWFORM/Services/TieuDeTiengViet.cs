using System.Windows.Forms;

namespace HotelManager.Services
{
    public static class TieuDeTiengViet
    {
        // Đổi tên cột hiển thị; không sửa DataPropertyName dùng ánh xạ dữ liệu.
        public static void GanCho(DataGridView grid)
        {
            foreach (DataGridViewColumn col in grid.Columns)
            {
                var name = string.IsNullOrWhiteSpace(col.DataPropertyName) ? col.Name : col.DataPropertyName;
                col.HeaderText = name switch
                {
                    "PhongId" or "RoomId" => "Mã phòng",
                    "SoPhong" or "RoomNumber" => "Số phòng",
                    "TenLoaiPhong" or "RoomTypeName" => "Loại phòng",
                    "GiaPhong" or "PricePerNight" => "Giá / đêm",
                    "TrangThai" => "Trạng thái",
                    "Id" => "Mã",
                    "TenDichVu" => "Tên dịch vụ",
                    "DonGia" => "Đơn giá",
                    "SoLuong" => "Số lượng",
                    "ThanhTien" => "Thành tiền",
                    "NgaySuDung" => "Ngày sử dụng",
                    "DatPhongId" => "Mã đơn đặt",
                    "DichVuId" => "Mã dịch vụ",
                    "Username" => "Tên đăng nhập",
                    "FullName" => "Họ tên",
                    "Role" => "Quyền",
                    "IsActive" => "Trạng thái hoạt động",
                    "TenKhachHang" => "Tên khách hàng",
                    "TongTien" => "Tổng tiền",
                    "NgayNhan" => "Ngày nhận phòng",
                    "NgayTra" => "Ngày trả phòng",
                    _ => col.HeaderText
                };
                if (name == "GiaPhong" || name == "DonGia" || name == "ThanhTien" || name == "TongTien")
                    col.DefaultCellStyle.Format = "N0";
            }
        }
    }
}
