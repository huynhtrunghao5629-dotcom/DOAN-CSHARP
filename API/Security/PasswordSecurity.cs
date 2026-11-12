using System.Security.Cryptography;

namespace QuanLyKhachSanAPI.Security;

public static class PasswordSecurity
{
    private const int Iterations = 120_000;
    private const int SaltSize = 16;
    private const int HashSize = 32;

    public static string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);
        return $"pbkdf2${Iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    public static bool Verify(string password, string? stored, out bool needsUpgrade)
    {
        needsUpgrade = false;
        if (string.IsNullOrEmpty(stored) || string.IsNullOrEmpty(password)) return false;
        if (!stored.StartsWith("pbkdf2$", StringComparison.Ordinal))
        {
            // Dữ liệu từ những phiên bản cũ; nâng cấp thành hash sau đăng nhập thành công.
            needsUpgrade = true;
            return CryptographicOperations.FixedTimeEquals(
                System.Text.Encoding.UTF8.GetBytes(password),
                System.Text.Encoding.UTF8.GetBytes(stored));
        }
        try
        {
            var parts = stored.Split('$');
            if (parts.Length != 4 || !int.TryParse(parts[1], out int iterations) ||
                iterations < 10_000 || iterations > 1_000_000) return false;
            var salt = Convert.FromBase64String(parts[2]);
            var expected = Convert.FromBase64String(parts[3]);
            var actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations,
                HashAlgorithmName.SHA256, expected.Length);
            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }
        catch (FormatException) { return false; }
    }
}
