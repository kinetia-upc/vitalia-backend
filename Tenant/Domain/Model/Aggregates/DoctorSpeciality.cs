namespace VitaliaBackend.Tenant.Domain.Model.Aggregates;

public class DoctorSpeciality
{
    public Guid DoctorId { get; private set; }
    public Guid SpecialityId { get; private set; }

    protected DoctorSpeciality()
    {
    }

    public DoctorSpeciality(Guid doctorId, Guid specialityId)
    {
        DoctorId = doctorId;
        SpecialityId = specialityId;
    }
}
