using VitaliaBackend.Iam.Domain.Model.Aggregates;
using VitaliaBackend.Tenant.Domain.Model.Aggregates;

namespace VitaliaBackend.Tests.Domain;

public class UserTests
{
    [Fact]
    public void Constructor_SetsAllProperties()
    {
        var id = Guid.NewGuid();

        var user = new User(
            id, "center-001",
            "John", "Doe", "Smith",
            "DNI", "12345678",
            new DateOnly(1990, 5, 15),
            "john@example.com", "hashed_password",
            "555-1234", "M", true,
            "123 Main St", "admin");

        Assert.Equal(id, user.Id);
        Assert.Equal("center-001", user.HealthcareCenterId);
        Assert.Equal("John", user.Name);
        Assert.Equal("Doe", user.PaternalSurname);
        Assert.Equal("Smith", user.MaternalSurname);
        Assert.Equal("DNI", user.IdentityType);
        Assert.Equal("12345678", user.IdentityNumber);
        Assert.Equal(new DateOnly(1990, 5, 15), user.BirthDate);
        Assert.Equal("john@example.com", user.Email);
        Assert.Equal("hashed_password", user.PasswordHash);
        Assert.Equal("555-1234", user.Phone);
        Assert.Equal("M", user.Gender);
        Assert.True(user.IsActive);
        Assert.Equal("123 Main St", user.Address);
        Assert.Equal("admin", user.Role);
    }

    [Fact]
    public void Constructor_EmptyId_GeneratesNewGuid()
    {
        var user = new User(
            Guid.Empty, "center-001",
            "John", "Doe", "Smith",
            "DNI", "12345678",
            new DateOnly(1990, 1, 1),
            "john@test.com", "hash",
            "555", "M", true,
            "Addr", "admin");

        Assert.NotEqual(Guid.Empty, user.Id);
    }
}

public class DoctorTests
{
    [Fact]
    public void Constructor_SetsAllProperties()
    {
        var userId = Guid.NewGuid();
        var doctor = new Doctor(userId, "DOC-001", "LIC-12345", "CMP-67890");

        Assert.Equal(userId, doctor.UserId);
        Assert.Equal("DOC-001", doctor.Code);
        Assert.Equal("LIC-12345", doctor.LicenseNumber);
        Assert.Equal("CMP-67890", doctor.CmpNumber);
    }
}

public class PatientTests
{
    [Fact]
    public void Constructor_SetsAllProperties()
    {
        var userId = Guid.NewGuid();
        var patient = new Patient(
            userId, "PAT-001", "Sura", "POL-123",
            new DateOnly(2026, 12, 31), "Emergency Contact", "555-9999");

        Assert.Equal(userId, patient.UserId);
        Assert.Equal("PAT-001", patient.Code);
        Assert.Equal("Sura", patient.InsuranceProvider);
        Assert.Equal("POL-123", patient.PolicyNumber);
        Assert.Equal(new DateOnly(2026, 12, 31), patient.ActiveThru);
        Assert.Equal("Emergency Contact", patient.EmergencyContactName);
        Assert.Equal("555-9999", patient.EmergencyContactPhone);
    }
}
