using VitaliaBackend.Clinical.Domain.Model.Aggregates;

namespace VitaliaBackend.Tests.Domain;

public class MedicalRecordTests
{
    [Fact]
    public void Constructor_GeneratesIdAndCode()
    {
        var apptId = Guid.NewGuid();
        var patientId = Guid.NewGuid();

        var mr = new MedicalRecord(apptId, patientId);

        Assert.NotEqual(Guid.Empty, mr.Id);
        Assert.StartsWith("HCE-", mr.Code);
        Assert.Equal(apptId, mr.AppointmentId);
        Assert.Equal(patientId, mr.PatientId);
    }

    [Fact]
    public void Constructor_WithId_SetsExactValues()
    {
        var id = Guid.NewGuid();
        var apptId = Guid.NewGuid();
        var patientId = Guid.NewGuid();

        var mr = new MedicalRecord(id, "HCE-99999", apptId, patientId);

        Assert.Equal(id, mr.Id);
        Assert.Equal("HCE-99999", mr.Code);
        Assert.Equal(apptId, mr.AppointmentId);
        Assert.Equal(patientId, mr.PatientId);
    }
}

public class PrescriptionTests
{
    [Fact]
    public void Constructor_GeneratesIdAndCode()
    {
        var mrId = Guid.NewGuid();

        var prs = new Prescription(mrId);

        Assert.NotEqual(Guid.Empty, prs.Id);
        Assert.StartsWith("PRS-", prs.Code);
        Assert.Equal(mrId, prs.MedicalRecordId);
    }

    [Fact]
    public void Constructor_WithId_SetsExactValues()
    {
        var id = Guid.NewGuid();
        var mrId = Guid.NewGuid();

        var prs = new Prescription(id, "PRS-12345", mrId);

        Assert.Equal(id, prs.Id);
        Assert.Equal("PRS-12345", prs.Code);
        Assert.Equal(mrId, prs.MedicalRecordId);
    }
}

public class PrescriptionDetailTests
{
    [Fact]
    public void Constructor_SetsAllFields()
    {
        var prsId = Guid.NewGuid();
        var medId = Guid.NewGuid();

        var detail = new PrescriptionDetail(prsId, medId, 10, 3, 7);

        Assert.Equal(prsId, detail.PrescriptionId);
        Assert.Equal(medId, detail.MedicineId);
        Assert.Equal(10, detail.Quantity);
        Assert.Equal(3, detail.Frequency);
        Assert.Equal(7, detail.Duration);
    }

    [Fact]
    public void UpdateDetails_ReplacesFields()
    {
        var detail = new PrescriptionDetail(Guid.NewGuid(), Guid.NewGuid(), 10, 3, 7);

        detail.UpdateDetails(20, 2, 14);

        Assert.Equal(20, detail.Quantity);
        Assert.Equal(2, detail.Frequency);
        Assert.Equal(14, detail.Duration);
    }
}

public class DiagnosisTests
{
    [Fact]
    public void Constructor_SetsAllFields()
    {
        var id = Guid.NewGuid();
        var mrId = Guid.NewGuid();

        var dx = new Diagnosis(id, "DX-001", mrId, "Gripe tipo A");

        Assert.Equal(id, dx.Id);
        Assert.Equal("DX-001", dx.Code);
        Assert.Equal(mrId, dx.MedicalRecordId);
        Assert.Equal("Gripe tipo A", dx.Description);
    }

    [Fact]
    public void UpdateDescription_ReplacesDescription()
    {
        var dx = new Diagnosis(Guid.NewGuid(), "DX-001", Guid.NewGuid(), "Old desc");

        dx.UpdateDescription("New description here");

        Assert.Equal("New description here", dx.Description);
    }
}

public class TreatmentTests
{
    [Fact]
    public void Constructor_SetsAllFields()
    {
        var id = Guid.NewGuid();
        var mrId = Guid.NewGuid();

        var tx = new Treatment(id, "TR-001", mrId, "Antibiotico 500mg");

        Assert.Equal(id, tx.Id);
        Assert.Equal("TR-001", tx.Code);
        Assert.Equal(mrId, tx.MedicalRecordId);
        Assert.Equal("Antibiotico 500mg", tx.Description);
    }

    [Fact]
    public void UpdateDescription_ReplacesDescription()
    {
        var tx = new Treatment(Guid.NewGuid(), "TR-001", Guid.NewGuid(), "Old");

        tx.UpdateDescription("Updated treatment");

        Assert.Equal("Updated treatment", tx.Description);
    }
}

public class MedicalOrderTests
{
    [Fact]
    public void Constructor_SetsAllProperties()
    {
        var id = Guid.NewGuid();
        var patientId = Guid.NewGuid();
        var doctorId = Guid.NewGuid();
        var apptId = Guid.NewGuid();
        var mrId = Guid.NewGuid();

        var order = new MedicalOrder(id, "ORD-001", patientId, doctorId, apptId, mrId, "Lab", "Blood test", "pending", "routine");

        Assert.Equal(id, order.Id);
        Assert.Equal("ORD-001", order.Code);
        Assert.Equal(patientId, order.PatientId);
        Assert.Equal(doctorId, order.DoctorId);
        Assert.Equal(apptId, order.AppointmentId);
        Assert.Equal(mrId, order.MedicalRecordId);
        Assert.Equal("Lab", order.Type);
        Assert.Equal("Blood test", order.Description);
        Assert.Equal("pending", order.Status);
        Assert.Equal("routine", order.Priority);
    }

    [Fact]
    public void Constructor_NullMedicalRecordId_SetsNull()
    {
        var order = new MedicalOrder(
            Guid.NewGuid(), "ORD-002", Guid.NewGuid(), Guid.NewGuid(),
            Guid.NewGuid(), null, "Lab", "Test", "pending", "routine");

        Assert.Null(order.MedicalRecordId);
    }

    [Fact]
    public void Constructor_EmptyId_GeneratesNewGuid()
    {
        var order = new MedicalOrder(
            Guid.Empty, "ORD-003", Guid.NewGuid(), Guid.NewGuid(),
            Guid.NewGuid(), null, "Lab", "Test", "pending", "routine");

        Assert.NotEqual(Guid.Empty, order.Id);
    }

    [Fact]
    public void Update_ReplacesEditableFields()
    {
        var order = new MedicalOrder(
            Guid.NewGuid(), "ORD-004", Guid.NewGuid(), Guid.NewGuid(),
            Guid.NewGuid(), null, "Lab", "Old desc", "pending", "routine");

        order.Update("Imaging", "New desc", "completed", "urgent");

        Assert.Equal("Imaging", order.Type);
        Assert.Equal("New desc", order.Description);
        Assert.Equal("completed", order.Status);
        Assert.Equal("urgent", order.Priority);
    }

    [Fact]
    public void Constructor_TrimsWhitespace()
    {
        var order = new MedicalOrder(
            Guid.NewGuid(), " ORD-005 ", Guid.NewGuid(), Guid.NewGuid(),
            Guid.NewGuid(), null, " Lab ", " Desc ", " pending ", " routine ");

        Assert.Equal("ORD-005", order.Code);
        Assert.Equal("Lab", order.Type);
        Assert.Equal("Desc", order.Description);
        Assert.Equal("pending", order.Status);
        Assert.Equal("routine", order.Priority);
    }
}
