using VitaliaBackend.Shared.Domain.Model.Entities;
namespace VitaliaBackend.Scheduling.Domain.Model.Entities;

public class SchedulingBranch : IAuditableEntity
{
    public int Id { get; private set; }
    public string PublicId { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    
    protected SchedulingBranch()
    {
        PublicId = string.Empty;
        Name = string.Empty;
        Description = string.Empty;
    }
    
    public SchedulingBranch(string publicId, string name, string description)
    {
        PublicId = publicId;
        Name = name;
        Description = description;
    }
    
    public void UpdateName(string name)
    {
        Name = name;
    }
    
    public void UpdateDescription(string description)
    {
        Description = description;
    }
}