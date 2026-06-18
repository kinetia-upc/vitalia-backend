namespace VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Shared.Domain.Model.Entities;

public class Diagnosis : IAuditableEntity
{
    public int Id { get; private set; }
    public string MedicalRecordId { get; private set; }
    public string Description { get; private set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    protected Diagnosis()
    {
        MedicalRecordId = string.Empty;
        Description = string.Empty;
    }
    
    public Diagnosis(string medicalRecordId, string description)
    {
        MedicalRecordId = medicalRecordId.Trim();
        Description = description.Trim();
    }

    public void UpdateDescription(string description)
    {
        Description = description.Trim();
    }
}
