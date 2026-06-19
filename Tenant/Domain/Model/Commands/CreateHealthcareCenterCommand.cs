namespace VitaliaBackend.Tenant.Domain.Model.Commands;

public record CreateHealthcareCenterCommand(
    string Id,
    string HealthcareCenterName,
    DateOnly? AllianceStartDate,
    DateOnly? AllianceFinishDate,
    long? RucNumber
);
