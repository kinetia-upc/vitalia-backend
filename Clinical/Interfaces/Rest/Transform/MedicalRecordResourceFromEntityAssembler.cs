using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Clinical.Interfaces.Rest.Resources;

namespace VitaliaBackend.Clinical.Interfaces.Rest.Transform;

public static class MedicalRecordResourceFromEntityAssembler
{
    public static MedicalRecordResource ToResourceFromEntity(MedicalRecord entity)
    {
        return new MedicalRecordResource(
            entity.Id,
            entity.Code,
            entity.PatientId,
            entity.AppointmentId,
            entity.CreatedAt,
            entity.UpdatedAt);
    }
}
