namespace VitaliaBackend.Tenant.Domain.Model.Commands;

public record CreateAppointmentFeeCommand(
    string Id,
    string BranchId,
    string? SpecialityId,
    decimal Price
);
