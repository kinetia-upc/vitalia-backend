using VitaliaBackend.Shared.Domain.Model.Entities;

namespace VitaliaBackend.Clinical.Domain.Model.Aggregates;

public class MedicalOrder : IAuditableEntity
{
    public Guid Id { get; private set; }
    public string Code { get; private set; }
    public Guid PatientId { get; private set; }
    public Guid DoctorId { get; private set; }
    public Guid AppointmentId { get; private set; }
    public Guid? MedicalRecordId { get; private set; }
    public string Type { get; private set; }
    public string Description { get; private set; }
    public string Status { get; private set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    protected MedicalOrder()
    {
        Code = string.Empty;
        Type = string.Empty;
        Description = string.Empty;
        Status = string.Empty;
    }

    public MedicalOrder(Guid id, string code, Guid patientId, Guid doctorId, Guid appointmentId, Guid? medicalRecordId, string type, string description, string status)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        Code = code.Trim();
        PatientId = patientId;
        DoctorId = doctorId;
        AppointmentId = appointmentId;
        MedicalRecordId = medicalRecordId;
        Type = type.Trim();
        Description = description.Trim();
        Status = status.Trim();
    }

    public void Update(string type, string description, string status)
    {
        Type = type.Trim();
        Description = description.Trim();
        Status = status.Trim();
    }
}
