namespace VitaliaBackend.Tenant.Domain.Model.Aggregates;

public class Patient
{
    public Guid UserId { get; private set; }
    public string Code { get; private set; }
    public string EHRCode { get; private set; }
    public string InsuranceProvider { get; private set; }
    public string PolicyNumber { get; private set; }
    public DateOnly? ActiveThru { get; private set; }
    public string EmergencyContactName { get; private set; }
    public string EmergencyContactPhone { get; private set; }

    protected Patient()
    {
        Code = string.Empty;
        EHRCode = string.Empty;
        InsuranceProvider = string.Empty;
        PolicyNumber = string.Empty;
        EmergencyContactName = string.Empty;
        EmergencyContactPhone = string.Empty;
    }

    public Patient(Guid userId, string code, string insuranceProvider, string policyNumber, DateOnly? activeThru, string emergencyContactName, string emergencyContactPhone, string? ehrCode = null)
    {
        UserId = userId;
        Code = code.Trim();
        EHRCode = string.IsNullOrWhiteSpace(ehrCode) ? GenerateEhrCode(Code) : ehrCode.Trim();
        InsuranceProvider = insuranceProvider.Trim();
        PolicyNumber = policyNumber.Trim();
        ActiveThru = activeThru;
        EmergencyContactName = emergencyContactName.Trim();
        EmergencyContactPhone = emergencyContactPhone.Trim();
    }

    private static string GenerateEhrCode(string patientCode)
    {
        var digits = new string(patientCode.Where(char.IsDigit).ToArray());
        var numericPortion = int.TryParse(digits, out var value) ? value : 0;
        return $"EHR-{numericPortion + 10000:D5}";
    }
}
