using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Clinical.Interfaces.Rest.Resources;

namespace VitaliaBackend.Clinical.Interfaces.Rest.Transform;

public static class PrescriptionDetailResourceFromEntityAssembler
{
    public static PrescriptionDetailResource ToResourceFromEntity(PrescriptionDetail entity)
    {
        return new PrescriptionDetailResource(
            entity.Id,
            entity.PrescriptionId,
            entity.MedicineId,
            entity.MedicineName,
            entity.Dose.Amount,
            entity.Dose.Unit.ToString(),
            entity.Frequency,
            entity.Duration,
            entity.CreatedAt,
            entity.UpdatedAt);
    }
}
