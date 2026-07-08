namespace VitaliaBackend.Tenant.Domain.Model.Aggregates;

public class Doctor
{
    public Guid UserId { get; private set; }
    public string Code { get; private set; }
    public string LicenseNumber { get; private set; }
    public string CmpNumber { get; private set; }

    protected Doctor()
    {
        Code = string.Empty;
        LicenseNumber = string.Empty;
        CmpNumber = string.Empty;
    }

    public Doctor(Guid userId, string code, string licenseNumber, string cmpNumber)
    {
        UserId = userId;
        Code = code.Trim();
        LicenseNumber = licenseNumber.Trim();
        CmpNumber = cmpNumber.Trim();
    }

    public void UpdateDetails(string licenseNumber, string cmpNumber)
    {
        LicenseNumber = licenseNumber.Trim();
        CmpNumber = cmpNumber.Trim();
    }
}
