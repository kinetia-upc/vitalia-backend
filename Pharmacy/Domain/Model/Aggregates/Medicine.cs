using VitaliaBackend.Shared.Domain.Model.Entities;

namespace VitaliaBackend.Pharmacy.Domain.Model.Aggregates;

public class Medicine : IAuditableEntity
{
    public Guid Id { get; private set; }
    public string Code { get; private set; }
    public string Name { get; private set; }
    public int UnitQuantity { get; private set; }
    public string UnitType { get; private set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    protected Medicine()
    {
        Code = string.Empty;
        Name = string.Empty;
        UnitType = string.Empty;
    }

    public Medicine(Guid id, string code, string name, int unitQuantity, string unitType)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        Code = code.Trim();
        Name = name.Trim();
        UnitQuantity = unitQuantity;
        UnitType = unitType.Trim();
    }

    public void UpdateDetails(string code, string name, int unitQuantity, string unitType)
    {
        Code = code.Trim();
        Name = name.Trim();
        UnitQuantity = unitQuantity;
        UnitType = unitType.Trim();
    }
}
