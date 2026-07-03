namespace VitaliaBackend.Tenant.Domain.Model.Commands;

public record UpdateAppointmentFeeCommand(
    Guid AppointmentFeeId,
    string BranchId,
    string? SpecialityId,
    decimal Price
);
