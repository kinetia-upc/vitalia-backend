using VitaliaBackend.Scheduling.Domain.Model.Commands;
using VitaliaBackend.Scheduling.Interfaces.Rest.Resources;

namespace VitaliaBackend.Scheduling.Interfaces.Rest.Transform;

public static class CreateAppointmentCommandFromResourceAssembler
{
    public static CreateAppointmentCommand ToCommandFromResource(CreateAppointmentResource resource)
    {
        return new CreateAppointmentCommand(
            resource.Code,
            resource.DoctorId,
            resource.PatientId,
            resource.BranchId,
            resource.ScheduledAt,
            resource.Reason,
            resource.Status,
            resource.PaymentStatus);
    }
}
