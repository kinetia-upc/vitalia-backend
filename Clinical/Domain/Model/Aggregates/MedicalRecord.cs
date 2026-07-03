using VitaliaBackend.Shared.Domain.Model.Entities;

namespace VitaliaBackend.Clinical.Domain.Model.Aggregates;

public class MedicalRecord : IAuditableEntity
{
    public Guid Id { get; private set; }
    public string Code { get; private set; }
    public Guid AppointmentId { get; private set; }
    public Guid PatientId { get; private set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    protected MedicalRecord()
    {
        Code = string.Empty;
    }

    public MedicalRecord(Guid appointmentId, Guid patientId)
    {
        Id = Guid.NewGuid();
        Code = GenerateCode();
        AppointmentId = appointmentId;
        PatientId = patientId;
    }

    public MedicalRecord(Guid id, string code, Guid appointmentId, Guid patientId)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        Code = code.Trim();
        AppointmentId = appointmentId;
        PatientId = patientId;
    }

    private static string GenerateCode()
    {
        return $"HCE-{Random.Shared.Next(10000, 100000)}";
    }
}
