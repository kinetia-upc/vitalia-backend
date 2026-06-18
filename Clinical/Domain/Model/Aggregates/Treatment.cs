namespace VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Shared.Domain.Model.Entities;

public class Treatment : IAuditableEntity
{
    public int Id { get; private set; }
    public string Description { get; private set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    protected Treatment()
    {
        Description = string.Empty;
    }
    
    public Treatment(string description)
    {
        Description = description;
    }
}