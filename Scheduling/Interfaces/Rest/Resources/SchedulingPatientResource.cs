namespace VitaliaBackend.Scheduling.Interfaces.Rest.Resources;

public record SchedulingPatientResource(
    string Id,
    string IdUser,
    string InsuranceProvider
    );