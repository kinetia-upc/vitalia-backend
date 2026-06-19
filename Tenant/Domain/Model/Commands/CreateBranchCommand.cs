namespace VitaliaBackend.Tenant.Domain.Model.Commands;

public record CreateBranchCommand(
    string Id,
    string HealthcareCenterId,
    string? AddressId,
    string BranchName,
    string Address
);
