namespace VitaliaBackend.Tenant.Domain.Model.Aggregates;

public class Patient
{
    public Guid UserId { get; private set; }
    public string Code { get; private set; }
    public string InsuranceProvider { get; private set; }
    public string PolicyNumber { get; private set; }
    public DateOnly? ActiveThru { get; private set; }
    public string EmergencyContactName { get; private set; }
    public string EmergencyContactPhone { get; private set; }

    protected Patient()
    {
        Code = string.Empty;
        InsuranceProvider = string.Empty;
        PolicyNumber = string.Empty;
        EmergencyContactName = string.Empty;
        EmergencyContactPhone = string.Empty;
    }

    public Patient(Guid userId, string code, string insuranceProvider, string policyNumber, DateOnly? activeThru, string emergencyContactName, string emergencyContactPhone)
    {
        UserId = userId;
        Code = code.Trim();
        InsuranceProvider = insuranceProvider.Trim();
        PolicyNumber = policyNumber.Trim();
        ActiveThru = activeThru;
        EmergencyContactName = emergencyContactName.Trim();
        EmergencyContactPhone = emergencyContactPhone.Trim();
    }
}
