using VitaliaBackend.Scheduling.Domain.Model.Aggregates;
using VitaliaBackend.Scheduling.Interfaces.Rest.Resources;

namespace VitaliaBackend.Scheduling.Interfaces.Rest.Transform;

public static class AppointmentResourceFromEntityAssembler
{
    public static AppointmentResource ToResourceFromEntity(Appointment entity)
    {
        return new AppointmentResource(
            entity.PublicId,
            entity.DoctorId,
            entity.PatientId,
            entity.BranchId,
            entity.ScheduledAt,
            entity.Reason,
            entity.Status.ToString(),
            entity.PaymentStatus.ToString());
    }
}