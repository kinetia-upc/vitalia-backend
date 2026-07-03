using VitaliaBackend.Tenant.Domain.Model.Commands;
using VitaliaBackend.Tenant.Interfaces.Rest.Resources;

namespace VitaliaBackend.Tenant.Interfaces.Rest.Transform;

public static class CreateAppointmentFeeCommandFromResourceAssembler
{
    public static CreateAppointmentFeeCommand ToCommandFromResource(CreateAppointmentFeeResource resource)
    {
        return new CreateAppointmentFeeCommand(
            resource.Code,
            resource.BranchId,
            resource.SpecialityId,
            resource.Price);
    }
}
