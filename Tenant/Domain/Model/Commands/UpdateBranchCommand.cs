namespace VitaliaBackend.Tenant.Domain.Model.Commands;

public record UpdateBranchCommand(
    string BranchId,
    string HealthcareCenterId,
    string BranchName,
    string Address
);
