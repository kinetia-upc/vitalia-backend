using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace VitaliaBackend.Iam.Infrastructure.Security;

public static class JwtTokenValidator
{
    public static bool TryValidate(string token, IConfiguration configuration, out Guid userId, out string role)
    {
        userId = Guid.Empty;
        role = string.Empty;

        var parts = token.Split('.');
        if (parts.Length != 3) return false;

        var secret = configuration["TokenSettings:Secret"] ?? "VitaliaDevelopmentSecretKey-ChangeBeforeProduction-2026";
        var unsignedToken = $"{parts[0]}.{parts[1]}";
        var expectedSignature = Base64UrlEncode(HMACSHA256.HashData(Encoding.UTF8.GetBytes(secret), Encoding.UTF8.GetBytes(unsignedToken)));

        if (!CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(parts[2]), Encoding.UTF8.GetBytes(expectedSignature)))
            return false;

        try
        {
            var payloadBytes = Base64UrlDecode(parts[1]);
            using var payload = JsonDocument.Parse(payloadBytes);
            var root = payload.RootElement;

            if (!root.TryGetProperty("sub", out var subProp) || !Guid.TryParse(subProp.GetString(), out userId))
                return false;

            role = root.TryGetProperty("role", out var roleProp) ? roleProp.GetString() ?? string.Empty : string.Empty;

            if (!root.TryGetProperty("exp", out var expProp))
                return false;

            var exp = expProp.GetInt64();
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds() < exp;
        }
        catch
        {
            userId = Guid.Empty;
            role = string.Empty;
            return false;
        }
    }

    private static byte[] Base64UrlDecode(string value)
    {
        var padded = value.Replace('-', '+').Replace('_', '/');
        padded = padded.PadRight(padded.Length + (4 - padded.Length % 4) % 4, '=');
        return Convert.FromBase64String(padded);
    }

    private static string Base64UrlEncode(byte[] bytes)
    {
        return Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }
}
