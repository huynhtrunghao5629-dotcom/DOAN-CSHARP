namespace QuanLyKhachSanAPI.Security;

public sealed class JwtSigningSettings
{
    public required byte[] Key { get; init; }
    public string Issuer { get; init; } = "QuanLyKhachSan";
    public string Audience { get; init; } = "QuanLyKhachSanUser";
}
