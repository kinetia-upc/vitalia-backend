using Microsoft.Extensions.Configuration;
using VitaliaBackend.Iam.Infrastructure.Security;

namespace VitaliaBackend.Tests.Security;

public class JwtTokenServiceTests
{
    private static IConfiguration CreateConfig()
    {
        var dict = new Dictionary<string, string?>
        {
            { "TokenSettings:Secret", "TestSecretKey-ForTestingOnly-2026" }
        };
        return new ConfigurationBuilder().AddInMemoryCollection(dict).Build();
    }

    private static VitaliaBackend.Iam.Domain.Model.Aggregates.User CreateTestUser()
    {
        return new VitaliaBackend.Iam.Domain.Model.Aggregates.User(
            Guid.NewGuid(), "center-001",
            "Test", "User", "X",
            "DNI", "12345678",
            new DateOnly(1990, 1, 1),
            "test@example.com", "hash",
            "555", "M", true,
            "Addr", "admin");
    }

    [Fact]
    public void Generate_ReturnsThreePartToken()
    {
        var config = CreateConfig();
        var user = CreateTestUser();

        var token = JwtTokenService.Generate(user, config);

        var parts = token.Split('.');
        Assert.Equal(3, parts.Length);
    }

    [Fact]
    public void Generate_ProducesValidBase64Url()
    {
        var config = CreateConfig();
        var user = CreateTestUser();

        var token = JwtTokenService.Generate(user, config);
        var parts = token.Split('.');

        foreach (var part in parts)
        {
            Assert.DoesNotContain("+", part);
            Assert.DoesNotContain("/", part);
            Assert.DoesNotContain("=", part);
        }
    }
}
