using VitaliaBackend.Shared.Domain.Model.Entities;

namespace VitaliaBackend.Clinical.Domain.Model.Aggregates;

public class Prescription : IAuditableEntity
{
    public Guid Id { get; private set; }
    public string Code { get; private set; }
    public Guid MedicalRecordId { get; private set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    protected Prescription()
    {
        Code = string.Empty;
    }

    public Prescription(Guid medicalRecordId)
    {
        Id = Guid.NewGuid();
        Code = GenerateCode();
        MedicalRecordId = medicalRecordId;
    }

    public Prescription(Guid id, string code, Guid medicalRecordId)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        Code = code.Trim();
        MedicalRecordId = medicalRecordId;
    }

    private static string GenerateCode()
    {
        return $"PRS-{Random.Shared.Next(10000, 100000)}";
    }
}
