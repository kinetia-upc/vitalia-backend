using VitaliaBackend.Scheduling.Domain.Model.Entities;
using VitaliaBackend.Scheduling.Interfaces.Rest.Resources;

namespace VitaliaBackend.Scheduling.Interfaces.Rest.Transform;

public static class SchedulingPatientResourceFromEntityAssembler
{
    public static SchedulingPatientResource ToResourceFromEntity(SchedulingPatient entity)
    {
        return new SchedulingPatientResource(
            entity.PublicId,
            entity.IdUser,
            entity.InsuranceProvider);
    }
}