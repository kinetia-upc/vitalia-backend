using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Clinical.Interfaces.Rest.Resources;

namespace VitaliaBackend.Clinical.Interfaces.Rest.Transform;

public static class TreatmentResourceFromEntityAssembler
{
    public static TreatmentResource ToResourceFromEntity(Treatment entity)
    {
        return new TreatmentResource(
            entity.Id,
            entity.MedicalRecordId,
            entity.Description,
            entity.CreatedAt,
            entity.UpdatedAt);
    }
}
