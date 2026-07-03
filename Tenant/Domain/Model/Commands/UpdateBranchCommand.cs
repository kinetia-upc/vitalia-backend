namespace VitaliaBackend.Tenant.Domain.Model.Commands;

public record UpdateBranchCommand(
    Guid BranchId,
    string HealthcareCenterId,
    string BranchName,
    string Address
);
