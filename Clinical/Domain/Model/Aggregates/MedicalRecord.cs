using VitaliaBackend.Shared.Domain.Model.Entities;

namespace VitaliaBackend.Clinical.Domain.Model.Aggregates;

public class MedicalRecord : IAuditableEntity
{
    public int Id { get; private set; }
    public string Code { get; private set; }
    public string AppointmentId { get; private set; }
    public string PatientId { get; private set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    protected MedicalRecord()
    {
        Code = string.Empty;
        AppointmentId = string.Empty;
        PatientId = string.Empty;
    }
    
    public MedicalRecord(string appointmentId, string patientId)
    {
        Code = GenerateCode();
        AppointmentId = appointmentId.Trim();
        PatientId = patientId.Trim();
    }

    private static string GenerateCode()
    {
        return $"HCE-{Random.Shared.Next(10000, 100000)}";
    }
}
