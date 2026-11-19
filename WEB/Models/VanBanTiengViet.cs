using System.Globalization;
using System.Text;

namespace QuanLyKhachSan.Web.Models;

/// <summary>
/// Chuẩn hóa văn bản để tìm kiếm không phân biệt chữ hoa/thường và dấu tiếng Việt.
/// Chỉ phục vụ việc hiển thị, tìm kiếm; không thay đổi dữ liệu trong Database.
/// </summary>
public static class VanBanTiengViet
{
    public static string BoDau(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;

        var decomposed = text.Normalize(NormalizationForm.FormD);
        var result = new StringBuilder(decomposed.Length);
        foreach (char c in decomposed)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark)
                continue;

            // Unicode không phân rã chữ Đ/đ thành D/d khi dùng FormD.
            result.Append(c is 'đ' ? 'd' : c is 'Đ' ? 'D' : c);
        }

        return result.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();
    }

    public static bool ChuaTuKhoa(string? noiDung, string? tuKhoa)
    {
        if (string.IsNullOrWhiteSpace(tuKhoa)) return true;
        return BoDau(noiDung).Contains(BoDau(tuKhoa), StringComparison.Ordinal);
    }
}
