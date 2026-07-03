using VitaliaBackend.Tenant.Domain.Model.Aggregates;

namespace VitaliaBackend.Tests.Domain;

public class HealthcareCenterTests
{
    [Fact]
    public void Constructor_PublicId_SetsAllProperties()
    {
        var start = new DateOnly(2025, 1, 1);
        var finish = new DateOnly(2027, 12, 31);

        var hc = new HealthcareCenter("HC-001", "Clínica Central", start, finish, "20456789012");

        Assert.NotEqual(Guid.Empty, hc.Id);
        Assert.Equal("HC-001", hc.Code);
        Assert.Equal("Clínica Central", hc.Name);
        Assert.Equal(start, hc.AllianceStartDate);
        Assert.Equal(finish, hc.AllianceFinishDate);
        Assert.Equal("20456789012", hc.RucNumber);
    }

    [Fact]
    public void Constructor_WithId_SetsExactValues()
    {
        var id = Guid.NewGuid();
        var hc = new HealthcareCenter(id, "HC-002", "Clínica Sur", null, null, null);

        Assert.Equal(id, hc.Id);
        Assert.Equal("HC-002", hc.Code);
        Assert.Equal("Clínica Sur", hc.Name);
        Assert.Null(hc.AllianceStartDate);
        Assert.Null(hc.AllianceFinishDate);
        Assert.Null(hc.RucNumber);
    }

    [Fact]
    public void Constructor_EmptyId_GeneratesNewGuid()
    {
        var hc = new HealthcareCenter(Guid.Empty, "HC-003", "Test", null, null, null);

        Assert.NotEqual(Guid.Empty, hc.Id);
    }

    [Fact]
    public void UpdateDetails_ReplacesEditableFields()
    {
        var hc = new HealthcareCenter("HC-004", "Old Name", null, null, null);

        hc.UpdateDetails("New Name", new DateOnly(2025, 6, 1), new DateOnly(2028, 6, 1), "20456789999");

        Assert.Equal("New Name", hc.Name);
        Assert.Equal(new DateOnly(2025, 6, 1), hc.AllianceStartDate);
        Assert.Equal(new DateOnly(2028, 6, 1), hc.AllianceFinishDate);
        Assert.Equal("20456789999", hc.RucNumber);
    }

    [Fact]
    public void Constructor_TrimsWhitespace()
    {
        var hc = new HealthcareCenter(" HC-005 ", " Trimmed Name ", null, null, null);

        Assert.Equal("HC-005", hc.Code);
        Assert.Equal("Trimmed Name", hc.Name);
    }
}

public class BranchTests
{
    [Fact]
    public void Constructor_PublicId_SetsAllProperties()
    {
        var branch = new Branch("BR-001", "HC-001", "Sede San Borja", "Av. San Borja 123");

        Assert.NotEqual(Guid.Empty, branch.Id);
        Assert.Equal("BR-001", branch.Code);
        Assert.Equal("HC-001", branch.HealthcareCenterId);
        Assert.Equal("Sede San Borja", branch.Name);
        Assert.Equal("Av. San Borja 123", branch.Address);
    }

    [Fact]
    public void Constructor_WithId_SetsExactValues()
    {
        var id = Guid.NewGuid();
        var branch = new Branch(id, "BR-002", "HC-001", "Sede Miraflores", "Calle 123");

        Assert.Equal(id, branch.Id);
        Assert.Equal("BR-002", branch.Code);
        Assert.Equal("HC-001", branch.HealthcareCenterId);
    }

    [Fact]
    public void Constructor_EmptyId_GeneratesNewGuid()
    {
        var branch = new Branch(Guid.Empty, "BR-003", "HC-001", "Name", "Addr");

        Assert.NotEqual(Guid.Empty, branch.Id);
    }

    [Fact]
    public void UpdateDetails_ReplacesEditableFields()
    {
        var branch = new Branch("BR-004", "HC-001", "Old Name", "Old Address");

        branch.UpdateDetails("HC-002", "New Name", "New Address");

        Assert.Equal("HC-002", branch.HealthcareCenterId);
        Assert.Equal("New Name", branch.Name);
        Assert.Equal("New Address", branch.Address);
    }

    [Fact]
    public void Constructor_TrimsWhitespace()
    {
        var branch = new Branch(" BR-006 ", " HC-001 ", " Name ", " Address ");

        Assert.Equal("BR-006", branch.Code);
        Assert.Equal("HC-001", branch.HealthcareCenterId);
        Assert.Equal("Name", branch.Name);
        Assert.Equal("Address", branch.Address);
    }
}

public class SpecialityTests
{
    [Fact]
    public void Constructor_SetsAllProperties()
    {
        var id = Guid.NewGuid();
        var spec = new Speciality(id, "SP-001", "Cardiología");

        Assert.Equal(id, spec.Id);
        Assert.Equal("SP-001", spec.Code);
        Assert.Equal("Cardiología", spec.Description);
    }

    [Fact]
    public void Constructor_EmptyId_GeneratesNewGuid()
    {
        var spec = new Speciality(Guid.Empty, "SP-002", "Pediatría");

        Assert.NotEqual(Guid.Empty, spec.Id);
    }

    [Fact]
    public void Constructor_TrimsWhitespace()
    {
        var spec = new Speciality(Guid.NewGuid(), " SP-003 ", " Dermatología ");

        Assert.Equal("SP-003", spec.Code);
        Assert.Equal("Dermatología", spec.Description);
    }
}

public class DoctorSpecialityTests
{
    [Fact]
    public void Constructor_SetsBothIds()
    {
        var doctorId = Guid.NewGuid();
        var specId = Guid.NewGuid();

        var ds = new DoctorSpeciality(doctorId, specId);

        Assert.Equal(doctorId, ds.DoctorId);
        Assert.Equal(specId, ds.SpecialityId);
    }
}

public class AppointmentFeeTests
{
    [Fact]
    public void Constructor_PublicId_SetsAllProperties()
    {
        var fee = new AppointmentFee("FEE-001", "BR-001", "SP-001", 150.50m);

        Assert.NotEqual(Guid.Empty, fee.Id);
        Assert.Equal("FEE-001", fee.Code);
        Assert.Equal("BR-001", fee.BranchId);
        Assert.Equal("SP-001", fee.SpecialityId);
        Assert.Equal(150.50m, fee.Price);
    }

    [Fact]
    public void Constructor_WithId_SetsExactValues()
    {
        var id = Guid.NewGuid();
        var fee = new AppointmentFee(id, "FEE-002", "BR-001", "SP-002", 200m);

        Assert.Equal(id, fee.Id);
        Assert.Equal("FEE-002", fee.Code);
    }

    [Fact]
    public void Constructor_EmptyId_GeneratesNewGuid()
    {
        var fee = new AppointmentFee(Guid.Empty, "FEE-003", "BR-001", "SP-001", 100m);

        Assert.NotEqual(Guid.Empty, fee.Id);
    }

    [Fact]
    public void Constructor_NullSpecialityId_SetsNull()
    {
        var fee = new AppointmentFee("FEE-004", "BR-001", null, 100m);

        Assert.Null(fee.SpecialityId);
    }

    [Fact]
    public void UpdateDetails_ReplacesEditableFields()
    {
        var fee = new AppointmentFee("FEE-005", "BR-001", "SP-001", 100m);

        fee.UpdateDetails("BR-002", "SP-003", 250m);

        Assert.Equal("BR-002", fee.BranchId);
        Assert.Equal("SP-003", fee.SpecialityId);
        Assert.Equal(250m, fee.Price);
    }

    [Fact]
    public void UpdateDetails_NullSpecialityId_SetsNull()
    {
        var fee = new AppointmentFee("FEE-006", "BR-001", "SP-001", 100m);

        fee.UpdateDetails("BR-001", null, 100m);

        Assert.Null(fee.SpecialityId);
    }

    [Fact]
    public void Constructor_TrimsWhitespace()
    {
        var fee = new AppointmentFee(" FEE-007 ", " BR-001 ", " SP-001 ", 100m);

        Assert.Equal("FEE-007", fee.Code);
        Assert.Equal("BR-001", fee.BranchId);
        Assert.Equal("SP-001", fee.SpecialityId);
    }
}
