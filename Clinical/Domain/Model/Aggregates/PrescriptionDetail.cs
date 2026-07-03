using VitaliaBackend.Shared.Domain.Model.Entities;

namespace VitaliaBackend.Clinical.Domain.Model.Aggregates;

public class PrescriptionDetail : IAuditableEntity
{
    public Guid PrescriptionId { get; private set; }
    public Guid MedicineId { get; private set; }
    public int Quantity { get; private set; }
    public int Frequency { get; private set; }
    public int Duration { get; private set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    protected PrescriptionDetail()
    {
    }

    public PrescriptionDetail(
        Guid prescriptionId, Guid medicineId, int quantity, int frequency, int duration)
    {
        PrescriptionId = prescriptionId;
        MedicineId = medicineId;
        Quantity = quantity;
        Frequency = frequency;
        Duration = duration;
    }

    public void UpdateDetails(
        int quantity,
        int frequency,
        int duration)
    {
        Quantity = quantity;
        Frequency = frequency;
        Duration = duration;
    }
}
