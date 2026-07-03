namespace VitaliaBackend.Pharmacy.Domain.Model.Aggregates;

public class BranchMedicine
{
    public string BranchId { get; private set; }
    public Guid MedicineId { get; private set; }
    public int Stock { get; private set; }
    public decimal Price { get; private set; }

    protected BranchMedicine()
    {
        BranchId = string.Empty;
    }

    public BranchMedicine(string branchId, Guid medicineId, int stock, decimal price)
    {
        BranchId = branchId.Trim();
        MedicineId = medicineId;
        Stock = stock;
        Price = price;
    }

    public bool HasEnoughStock(int quantity) => Stock >= quantity;

    public void IncreaseStock(int quantity)
    {
        Stock += quantity;
    }

    public bool TryDecreaseStock(int quantity)
    {
        if (!HasEnoughStock(quantity)) return false;
        Stock -= quantity;
        return true;
    }
}
