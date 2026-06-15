using VitaliaBackend.Scheduling.Domain.Model.Commands;
using VitaliaBackend.Scheduling.Interfaces.Rest.Resources;

namespace VitaliaBackend.Scheduling.Interfaces.Rest.Transform;

public static class UpdateAppointmentCommandFromResourceAssembler
{
    public static UpdateAppointmentScheduleCommand ToCommandFromResource(
        string appointmentId,
        UpdateAppointmentResource resource)
    {
        return new UpdateAppointmentScheduleCommand(
            appointmentId,
            resource.DoctorId,
            resource.PatientId,
            resource.BranchId,
            resource.ScheduledAt,
            resource.Reason,
            resource.Status,
            resource.PaymentStatus);
    }
}