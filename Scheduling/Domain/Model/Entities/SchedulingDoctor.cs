using VitaliaBackend.Shared.Domain.Model.Entities;
namespace VitaliaBackend.Scheduling.Domain.Model.Entities;

public class SchedulingDoctor : IAuditableEntity
{
    public int Id { get; private set; }
    public string PublicId { get; private set; }
    public string IdUser { get; private set; }
    public string Specialty { get; private set; }
    public string BranchId { get; private set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    
    protected SchedulingDoctor()
    {
        PublicId = string.Empty;
        IdUser = string.Empty;
        Specialty = string.Empty;
        BranchId = string.Empty;
    }
    
    public SchedulingDoctor(string publicId, string idUser, string specialty, string branchId)
    {
        PublicId = publicId;
        IdUser = idUser;
        Specialty = specialty;
        BranchId = branchId;
    }
    
    public void UpdateSpeciality(string specialty)
    {
        Specialty = specialty;
    }
    
    public void UpdateBranch(string branchId)
    {
        BranchId = branchId;
    }
}