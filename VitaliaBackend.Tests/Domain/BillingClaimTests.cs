using VitaliaBackend.Billing.Domain.Model.Aggregates;

namespace VitaliaBackend.Tests.Domain;

public class BillingClaimTests
{
    [Fact]
    public void Constructor_SetsAllProperties()
    {
        var claim = new BillingClaim(
            "CLM-2026-0001",
            Guid.NewGuid(),
            "Aseguradora XYZ",
            "Juan Perez",
            "Dr. Garcia",
            1500.00m,
            "verified",
            "In Clearinghouse");

        Assert.NotEqual(Guid.Empty, claim.Id);
        Assert.Equal("CLM-2026-0001", claim.Code);
        Assert.NotEqual(Guid.Empty, claim.AppointmentId);
        Assert.Equal("Aseguradora XYZ", claim.InsuranceProvider);
        Assert.Equal("Juan Perez", claim.PatientName);
        Assert.Equal("Dr. Garcia", claim.ProviderName);
        Assert.Equal(1500.00m, claim.Value);
        Assert.Equal("verified", claim.ClinicalCompliance);
        Assert.Equal("In Clearinghouse", claim.CycleStatus);
    }

    [Fact]
    public void Constructor_WithId_SetsId()
    {
        var id = Guid.NewGuid();
        var claim = new BillingClaim(id, "CLM-2026-0002", Guid.NewGuid(), "Ins", "Patient", "Provider", 100m, "pending_sign", "Funds Released");

        Assert.Equal(id, claim.Id);
    }

    [Fact]
    public void Constructor_EmptyId_GeneratesNewGuid()
    {
        var claim = new BillingClaim(Guid.Empty, "CLM-2026-0003", Guid.Empty, "Ins", "Patient", "Provider", 100m, "verified", "Rejected");

        Assert.NotEqual(Guid.Empty, claim.Id);
    }

    [Fact]
    public void UpdateDetails_ReplacesEditableFields()
    {
        var claim = new BillingClaim(
            "CLM-001", Guid.NewGuid(), "Ins1", "Patient1", "Provider1", 100m, "verified", "In Clearinghouse");

        var appointmentId = Guid.NewGuid();

        claim.UpdateDetails("CLM-002", appointmentId, "Ins2", "Patient2", "Provider2", 200m, "pending_sign", "Funds Released");

        Assert.Equal("CLM-002", claim.Code);
        Assert.Equal(appointmentId, claim.AppointmentId);
        Assert.Equal("Ins2", claim.InsuranceProvider);
        Assert.Equal("Patient2", claim.PatientName);
        Assert.Equal("Provider2", claim.ProviderName);
        Assert.Equal(200m, claim.Value);
        Assert.Equal("pending_sign", claim.ClinicalCompliance);
        Assert.Equal("Funds Released", claim.CycleStatus);
    }
}
