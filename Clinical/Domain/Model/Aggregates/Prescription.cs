using VitaliaBackend.Shared.Domain.Model.Entities;

namespace VitaliaBackend.Clinical.Domain.Model.Aggregates;

public class Prescription : IAuditableEntity
{
    public int Id { get; private set; }
    public string MedicalRecordId { get; private set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    protected Prescription()
    {
        MedicalRecordId = string.Empty;
    }

    public Prescription(string medicalRecordId)
    {
        this.MedicalRecordId = medicalRecordId;
    }
}
