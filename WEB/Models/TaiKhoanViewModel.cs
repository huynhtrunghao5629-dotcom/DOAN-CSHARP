using System.ComponentModel.DataAnnotations;

namespace QuanLyKhachSan.Web.Models
{
    // Model dùng cho form Đăng nhập
    public class DangNhapViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập Số điện thoại hoặc Email.")]
        public string TaiKhoan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        public string MatKhau { get; set; } = string.Empty;
    }

    // Model dùng cho form Đăng ký
    public class DangKyViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập Họ và tên.")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập Số điện thoại.")]
        [RegularExpression(@"^\d{10,11}$", ErrorMessage = "Số điện thoại phải từ 10-11 số.")]
        public string SoDienThoai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập CCCD/CMND.")]
        public string CCCD { get; set; } = string.Empty;

        public string? Email { get; set; } // Email có thể không bắt buộc

        [Required(ErrorMessage = "Vui lòng nhập Mật khẩu.")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        public string MatKhau { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng xác nhận Mật khẩu.")]
        [Compare("MatKhau", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        public string XacNhanMatKhau { get; set; } = string.Empty;
    }
}