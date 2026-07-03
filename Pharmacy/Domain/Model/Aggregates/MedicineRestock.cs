using VitaliaBackend.Shared.Domain.Model.Entities;

namespace VitaliaBackend.Pharmacy.Domain.Model.Aggregates;

public class MedicineRestock : IAuditableEntity
{
    public Guid Id { get; private set; }
    public string Code { get; private set; }
    public string BranchId { get; private set; }
    public Guid MedicineId { get; private set; }
    public int Quantity { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    protected MedicineRestock()
    {
        Code = string.Empty;
        BranchId = string.Empty;
    }

    public MedicineRestock(Guid id, string code, string branchId, Guid medicineId, int quantity, Guid createdByUserId)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        Code = code.Trim();
        BranchId = branchId.Trim();
        MedicineId = medicineId;
        Quantity = quantity;
        CreatedByUserId = createdByUserId;
    }
}
