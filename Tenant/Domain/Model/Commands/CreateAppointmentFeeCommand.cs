namespace VitaliaBackend.Tenant.Domain.Model.Commands;

public record CreateAppointmentFeeCommand(
    string? Code,
    string BranchId,
    string? SpecialityId,
    decimal Price
);
