using VitaliaBackend.Shared.Domain.Model.Entities;

namespace VitaliaBackend.Pharmacy.Domain.Model.Aggregates;

public class Medicine : IAuditableEntity
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public int UnitQuantity { get; private set; }
    public string UnitType { get; private set; }
    public decimal Price { get; private set; }
    public int Stock { get; private set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    protected Medicine()
    {
        Name = string.Empty;
        UnitType = string.Empty;
    }

    public Medicine(string name, int unitQuantity, string unitType, decimal price, int stock)
    {
        Name = name.Trim();
        UnitQuantity = unitQuantity;
        UnitType = unitType.Trim();
        Price = price;
        Stock = stock;
    }

    public void UpdateDetails(string name, int unitQuantity, string unitType, decimal price, int stock)
    {
        Name = name.Trim();
        UnitQuantity = unitQuantity;
        UnitType = unitType.Trim();
        Price = price;
        Stock = stock;
    }
}
