using VitaliaBackend.Scheduling.Domain.Model.Entities;
using VitaliaBackend.Scheduling.Interfaces.Rest.Resources;

namespace VitaliaBackend.Scheduling.Interfaces.Rest.Transform;

public static class SchedulingDoctorResourceFromEntityAssembler
{
    public static SchedulingDoctorResource ToResourceFromEntity(SchedulingDoctor entity)
    {
        return new SchedulingDoctorResource(
            entity.PublicId,
            entity.IdUser,
            entity.Specialty,
            entity.BranchId);
    }
}