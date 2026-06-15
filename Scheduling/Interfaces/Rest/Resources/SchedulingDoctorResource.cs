namespace VitaliaBackend.Scheduling.Interfaces.Rest.Resources;

public record SchedulingDoctorResource(
    string Id,
    string IdUser,
    string Specialty,
    string BranchId
    );