namespace VitaliaBackend.Tenant.Domain.Model.Commands;

public record UpdateBranchCommand(
    string BranchId,
    string HealthcareCenterId,
    string? AddressId,
    string BranchName,
    string Address
);
