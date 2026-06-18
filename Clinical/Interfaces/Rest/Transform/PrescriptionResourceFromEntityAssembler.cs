using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Clinical.Interfaces.Rest.Resources;

namespace VitaliaBackend.Clinical.Interfaces.Rest.Transform;

public static class PrescriptionResourceFromEntityAssembler
{
    public static PrescriptionResource ToResourceFromEntity(Prescription entity)
    {
        return new PrescriptionResource(
            entity.Id,
            entity.MedicalRecordId,
            entity.CreatedAt,
            entity.UpdatedAt);
    }
}
