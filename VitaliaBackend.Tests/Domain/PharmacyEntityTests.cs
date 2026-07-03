using VitaliaBackend.Pharmacy.Domain.Model.Aggregates;

namespace VitaliaBackend.Tests.Domain;

public class MedicineTests
{
    [Fact]
    public void Constructor_SetsAllProperties()
    {
        var id = Guid.NewGuid();
        var med = new Medicine(id, "MED-001", "Paracetamol 500mg", 20, "Tablets");

        Assert.Equal(id, med.Id);
        Assert.Equal("MED-001", med.Code);
        Assert.Equal("Paracetamol 500mg", med.Name);
        Assert.Equal(20, med.UnitQuantity);
        Assert.Equal("Tablets", med.UnitType);
    }

    [Fact]
    public void Constructor_EmptyId_GeneratesNewGuid()
    {
        var med = new Medicine(Guid.Empty, "MED-002", "Ibuprofeno", 10, "Capsules");

        Assert.NotEqual(Guid.Empty, med.Id);
    }

    [Fact]
    public void UpdateDetails_ReplacesAllEditableFields()
    {
        var med = new Medicine(Guid.NewGuid(), "MED-003", "Old Name", 10, "Old Type");

        med.UpdateDetails("MED-004", "New Name", 30, "New Type");

        Assert.Equal("MED-004", med.Code);
        Assert.Equal("New Name", med.Name);
        Assert.Equal(30, med.UnitQuantity);
        Assert.Equal("New Type", med.UnitType);
    }

    [Fact]
    public void UpdateDetails_TrimsWhitespace()
    {
        var med = new Medicine(Guid.NewGuid(), "MED-005", "Name", 10, "Type");

        med.UpdateDetails(" MED-006 ", " Trimmed Name ", 5, " Trimmed Type ");

        Assert.Equal("MED-006", med.Code);
        Assert.Equal("Trimmed Name", med.Name);
        Assert.Equal("Trimmed Type", med.UnitType);
    }
}

public class MedicineRestockTests
{
    [Fact]
    public void Constructor_SetsAllProperties()
    {
        var id = Guid.NewGuid();
        var medId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var restock = new MedicineRestock(id, "RSK-001", "brc-001", medId, 50, userId);

        Assert.Equal(id, restock.Id);
        Assert.Equal("RSK-001", restock.Code);
        Assert.Equal("brc-001", restock.BranchId);
        Assert.Equal(medId, restock.MedicineId);
        Assert.Equal(50, restock.Quantity);
        Assert.Equal(userId, restock.CreatedByUserId);
    }

    [Fact]
    public void Constructor_EmptyId_GeneratesNewGuid()
    {
        var restock = new MedicineRestock(Guid.Empty, "RSK-002", "brc-001", Guid.NewGuid(), 10, Guid.NewGuid());

        Assert.NotEqual(Guid.Empty, restock.Id);
    }

    [Fact]
    public void Constructor_TrimsWhitespace()
    {
        var restock = new MedicineRestock(
            Guid.NewGuid(), " RSK-003 ", " brc-002 ", Guid.NewGuid(), 25, Guid.NewGuid());

        Assert.Equal("RSK-003", restock.Code);
        Assert.Equal("brc-002", restock.BranchId);
    }
}
