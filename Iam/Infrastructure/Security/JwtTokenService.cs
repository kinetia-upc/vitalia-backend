using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using VitaliaBackend.Iam.Domain.Model.Aggregates;

namespace VitaliaBackend.Iam.Infrastructure.Security;

public static class JwtTokenService
{
    public static string Generate(User user, IConfiguration configuration)
    {
        var secret = configuration["TokenSettings:Secret"] ?? "VitaliaDevelopmentSecretKey-ChangeBeforeProduction-2026";
        var header = new Dictionary<string, object>
        {
            ["alg"] = "HS256",
            ["typ"] = "JWT"
        };
        var payload = new Dictionary<string, object>
        {
            ["sub"] = user.Id.ToString(),
            ["email"] = user.Email,
            ["role"] = user.Role,
            ["healthcareCenterId"] = user.HealthcareCenterId,
            ["exp"] = DateTimeOffset.UtcNow.AddDays(7).ToUnixTimeSeconds()
        };

        var encodedHeader = Base64UrlEncode(JsonSerializer.SerializeToUtf8Bytes(header));
        var encodedPayload = Base64UrlEncode(JsonSerializer.SerializeToUtf8Bytes(payload));
        var unsignedToken = $"{encodedHeader}.{encodedPayload}";
        var signature = Sign(unsignedToken, secret);
        return $"{unsignedToken}.{signature}";
    }

    private static string Sign(string value, string secret)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        return Base64UrlEncode(hmac.ComputeHash(Encoding.UTF8.GetBytes(value)));
    }

    private static string Base64UrlEncode(byte[] bytes)
    {
        return Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }
}
