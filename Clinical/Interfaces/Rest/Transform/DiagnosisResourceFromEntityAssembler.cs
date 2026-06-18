using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Clinical.Interfaces.Rest.Resources;

namespace VitaliaBackend.Clinical.Interfaces.Rest.Transform;

public static class DiagnosisResourceFromEntityAssembler
{
    public static DiagnosisResource ToResourceFromEntity(Diagnosis entity)
    {
        return new DiagnosisResource(
            entity.Id,
            entity.Description,
            entity.CreatedAt,
            entity.UpdatedAt);
    }
}
