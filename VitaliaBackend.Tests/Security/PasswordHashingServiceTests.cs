using VitaliaBackend.Iam.Infrastructure.Security;

namespace VitaliaBackend.Tests.Security;

public class PasswordHashingServiceTests
{
    [Fact]
    public void Hash_ProducesPbkdf2Format()
    {
        var hash = PasswordHashingService.Hash("TestPassword123!");

        Assert.StartsWith("PBKDF2$", hash);
        var parts = hash.Split('$');
        Assert.Equal(4, parts.Length);
        Assert.Equal("100000", parts[1]);
    }

    [Fact]
    public void Verify_ReturnsTrue_ForCorrectPassword()
    {
        var password = "SecureP@ssw0rd";
        var hash = PasswordHashingService.Hash(password);

        Assert.True(PasswordHashingService.Verify(password, hash));
    }

    [Fact]
    public void Verify_ReturnsFalse_ForWrongPassword()
    {
        var hash = PasswordHashingService.Hash("CorrectPassword");

        Assert.False(PasswordHashingService.Verify("WrongPassword", hash));
    }

    [Fact]
    public void Hash_ProducesDifferentHashes_ForSamePassword()
    {
        var hash1 = PasswordHashingService.Hash("SamePassword");
        var hash2 = PasswordHashingService.Hash("SamePassword");

        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void Verify_ReturnsFalse_ForCorruptedHash()
    {
        var hash = PasswordHashingService.Hash("Password");
        var corrupted = hash.Substring(0, hash.Length - 5) + "XXXXX";

        Assert.False(PasswordHashingService.Verify("Password", corrupted));
    }
}
