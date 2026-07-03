using VitaliaBackend.Tenant.Domain.Model.Aggregates;
using VitaliaBackend.Tenant.Interfaces.Rest.Resources;

namespace VitaliaBackend.Tenant.Interfaces.Rest.Transform;

public static class HealthcareCenterResourceFromEntityAssembler
{
    public static HealthcareCenterResource ToResourceFromEntity(HealthcareCenter entity)
    {
        return new HealthcareCenterResource(
            entity.Id,
            entity.Code,
            entity.Name,
            entity.AllianceStartDate,
            entity.AllianceFinishDate,
            entity.RucNumber);
    }
}
