namespace VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Shared.Domain.Model.Entities;

public class Diagnosis : IAuditableEntity
{
    public int Id { get; private set; }
    public string Description { get; private set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    protected Diagnosis()
    {
        Description = string.Empty;
    }
    
    public Diagnosis(string description)
    {
        Description = description;
    }
}