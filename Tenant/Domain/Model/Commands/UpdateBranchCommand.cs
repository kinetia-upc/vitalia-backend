namespace VitaliaBackend.Tenant.Domain.Model.Commands;

using VitaliaBackend.Shared.Domain.Model.ValueObjects;

public record UpdateBranchCommand(
    Guid BranchId,
    string HealthcareCenterId,
    string BranchName,
    string Address,
    DiagnosisCatalogSource DiagnosisCatalogSource = DiagnosisCatalogSource.MINSA_CIE10
);
