namespace VitaliaBackend.Iam.Interfaces.Rest.Resources;

public record UserResource(
    Guid Id,
    string HealthcareCenterId,
    string Name,
    string PaternalSurname,
    string MaternalSurname,
    string IdentityType,
    string IdentityNumber,
    DateOnly DateBirth,
    string Email,
    string Phone,
    string Gender,
    bool IsActive,
    string Address,
    string Role);
