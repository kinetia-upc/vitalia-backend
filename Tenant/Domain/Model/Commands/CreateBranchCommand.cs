namespace VitaliaBackend.Tenant.Domain.Model.Commands;

public record CreateBranchCommand(
    string Code,
    string HealthcareCenterId,
    string BranchName,
    string Address
);
