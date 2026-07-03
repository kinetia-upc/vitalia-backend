namespace VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Shared.Domain.Model.Entities;

public class Treatment : IAuditableEntity
{
    public Guid Id { get; private set; }
    public string Code { get; private set; }
    public Guid MedicalRecordId { get; private set; }
    public string Description { get; private set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    protected Treatment()
    {
        Code = string.Empty;
        Description = string.Empty;
    }
    
    public Treatment(Guid id, string code, Guid medicalRecordId, string description)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        Code = code.Trim();
        MedicalRecordId = medicalRecordId;
        Description = description.Trim();
    }

    public void UpdateDescription(string description)
    {
        Description = description.Trim();
    }
}
