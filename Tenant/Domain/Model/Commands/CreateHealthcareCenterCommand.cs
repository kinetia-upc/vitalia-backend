namespace VitaliaBackend.Tenant.Domain.Model.Commands;

public record CreateHealthcareCenterCommand(
    string Code,
    string HealthcareCenterName,
    DateOnly? AllianceStartDate,
    DateOnly? AllianceFinishDate,
    string? RucNumber,
    string? ImageUrl
);
