using VitaliaBackend.Shared.Domain.Model.Entities;
namespace VitaliaBackend.Scheduling.Domain.Model.Entities;

public class SchedulingPatient : IAuditableEntity
{
    public int Id { get; private set; }
    public string PublicId { get; private set; }
    public string IdUser { get; private set; }
    public string InsuranceProvider { get; private set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    
    protected SchedulingPatient()
    {
        PublicId = string.Empty;
        IdUser = string.Empty;
        InsuranceProvider = string.Empty;
    }
    
    public SchedulingPatient(string publicId, string idUser, string insuranceProvider)
    {
        PublicId = publicId;
        IdUser = idUser;
        InsuranceProvider = insuranceProvider;
    }
    
    public void UpdateInsuranceProvider(string insuranceProvider)
    {
        InsuranceProvider = insuranceProvider;
    }
}