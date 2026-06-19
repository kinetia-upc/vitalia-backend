using VitaliaBackend.Tenant.Domain.Model.Commands;
using VitaliaBackend.Tenant.Interfaces.Rest.Resources;

namespace VitaliaBackend.Tenant.Interfaces.Rest.Transform;

public static class UpdateAppointmentFeeCommandFromResourceAssembler
{
    public static UpdateAppointmentFeeCommand ToCommandFromResource(
        string appointmentFeeId,
        UpdateAppointmentFeeResource resource)
    {
        return new UpdateAppointmentFeeCommand(
            appointmentFeeId,
            resource.IdBranch,
            resource.IdSpeciality,
            resource.Price);
    }
}
