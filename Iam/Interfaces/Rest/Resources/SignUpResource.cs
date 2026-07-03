namespace VitaliaBackend.Iam.Interfaces.Rest.Resources;

public record SignUpResource(
    string HealthcareCenterId,
    string Name,
    string PaternalSurname,
    string MaternalSurname,
    string IdentityType,
    string IdentityNumber,
    DateOnly DateBirth,
    string Email,
    string Password,
    string Phone,
    string Gender,
    string Address,
    string Role);
