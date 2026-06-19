using VitaliaBackend.Tenant.Domain.Model.Aggregates;
using VitaliaBackend.Tenant.Interfaces.Rest.Resources;

namespace VitaliaBackend.Tenant.Interfaces.Rest.Transform;

public static class AppointmentFeeResourceFromEntityAssembler
{
    public static AppointmentFeeResource ToResourceFromEntity(AppointmentFee entity)
    {
        return new AppointmentFeeResource(
            entity.PublicId,
            entity.BranchId,
            entity.SpecialityId,
            entity.Price);
    }
}
