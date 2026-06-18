using VitaliaBackend.Clinical.Domain.Model.ValueObjects;
using VitaliaBackend.Pharmacy.Domain.Model.Aggregates;
using VitaliaBackend.Shared.Domain.Model.Entities;

namespace VitaliaBackend.Clinical.Domain.Model.Aggregates;

public class PrescriptionDetail : IAuditableEntity
{
    public int Id { get; private set; }
    
    public int PrescriptionId { get; private set; }
    public int? MedicineId { get; private set; }
    public string? MedicineName { get; private set; }
    public Dose Dose { get; private set; }
    public string Frequency { get; private set; }
    public string Duration { get; private set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    protected PrescriptionDetail()
    {
        MedicineName = null;
        MedicineId = null;
        Frequency = string.Empty;
        Duration = string.Empty;
        Dose = new Dose();
    }

    public PrescriptionDetail(
        int prescriptionId, int? medicineId, string? medicineName, Dose dose, string frequency, string duration)
    {
        PrescriptionId = prescriptionId;
        MedicineId = medicineId;
        MedicineName = medicineName;
        Dose = dose;
        Frequency = frequency;
        Duration = duration;
    }
}
