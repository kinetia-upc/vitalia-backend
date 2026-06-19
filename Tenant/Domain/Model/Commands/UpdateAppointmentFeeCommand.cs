namespace VitaliaBackend.Tenant.Domain.Model.Commands;

public record UpdateAppointmentFeeCommand(
    string AppointmentFeeId,
    string BranchId,
    string? SpecialityId,
    decimal Price
);
