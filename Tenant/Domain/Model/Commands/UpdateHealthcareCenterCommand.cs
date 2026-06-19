namespace VitaliaBackend.Tenant.Domain.Model.Commands;

public record UpdateHealthcareCenterCommand(
    string HealthcareCenterId,
    string HealthcareCenterName,
    DateOnly? AllianceStartDate,
    DateOnly? AllianceFinishDate,
    long? RucNumber
);
