using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Clinical.Interfaces.Rest.Resources;

namespace VitaliaBackend.Clinical.Interfaces.Rest.Transform;

public static class DiagnosisResourceFromEntityAssembler
{
    public static DiagnosisResource ToResourceFromEntity(Diagnosis entity)
    {
        return new DiagnosisResource(
            entity.Id,
            entity.Code,
            entity.MedicalRecordId,
            entity.Cie10Code,
            entity.Description,
            entity.DiagnosisCatalogSource,
            entity.CreatedAt,
            entity.UpdatedAt);
    }
}
