using VitaliaBackend.Pharmacy.Domain.Model.Aggregates;

namespace VitaliaBackend.Tests.Domain;

public class BranchMedicineTests
{
    [Fact]
    public void Constructor_SetsProperties()
    {
        var medicineId = Guid.NewGuid();
        var bm = new BranchMedicine("brc-00001", medicineId, 100, 25.50m);

        Assert.Equal("brc-00001", bm.BranchId);
        Assert.Equal(medicineId, bm.MedicineId);
        Assert.Equal(100, bm.Stock);
        Assert.Equal(25.50m, bm.Price);
    }

    [Fact]
    public void HasEnoughStock_ReturnsTrue_WhenStockSufficient()
    {
        var bm = new BranchMedicine("brc-00001", Guid.NewGuid(), 50, 10m);

        Assert.True(bm.HasEnoughStock(50));
        Assert.True(bm.HasEnoughStock(1));
    }

    [Fact]
    public void HasEnoughStock_ReturnsFalse_WhenStockInsufficient()
    {
        var bm = new BranchMedicine("brc-00001", Guid.NewGuid(), 5, 10m);

        Assert.False(bm.HasEnoughStock(10));
    }

    [Fact]
    public void IncreaseStock_AddsQuantity()
    {
        var bm = new BranchMedicine("brc-00001", Guid.NewGuid(), 20, 10m);

        bm.IncreaseStock(30);

        Assert.Equal(50, bm.Stock);
    }

    [Fact]
    public void TryDecreaseStock_DecreasesStock_WhenEnough()
    {
        var bm = new BranchMedicine("brc-00001", Guid.NewGuid(), 20, 10m);

        var result = bm.TryDecreaseStock(8);

        Assert.True(result);
        Assert.Equal(12, bm.Stock);
    }

    [Fact]
    public void TryDecreaseStock_ReturnsFalse_WhenInsufficient()
    {
        var bm = new BranchMedicine("brc-00001", Guid.NewGuid(), 3, 10m);

        var result = bm.TryDecreaseStock(10);

        Assert.False(result);
        Assert.Equal(3, bm.Stock);
    }
}
