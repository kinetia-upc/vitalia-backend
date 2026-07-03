using Microsoft.EntityFrameworkCore;
using VitaliaBackend.Pharmacy.Domain.Model.Aggregates;
using VitaliaBackend.Pharmacy.Domain.Repositories;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace VitaliaBackend.Pharmacy.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class MedicineRepository(AppDbContext context)
    : BaseRepository<Medicine>(context), IMedicineRepository
{
    public async Task<bool> ExistsByNameAndPresentationAsync(
        string name,
        int unitQuantity,
        string unitType,
        Guid? excludingId = null,
        CancellationToken cancellationToken = default)
    {
        var normalizedName = name.Trim().ToLower();
        var normalizedUnitType = unitType.Trim().ToLower();

        return await Context.Set<Medicine>()
            .AnyAsync(medicine =>
                    medicine.Name.ToLower() == normalizedName
                    && medicine.UnitQuantity == unitQuantity
                    && medicine.UnitType.ToLower() == normalizedUnitType
                    && (!excludingId.HasValue || medicine.Id != excludingId.Value),
                cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(
        string code,
        Guid? excludingId = null,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToLower();
        return await Context.Set<Medicine>()
            .AnyAsync(medicine =>
                    medicine.Code.ToLower() == normalizedCode
                    && (!excludingId.HasValue || medicine.Id != excludingId.Value),
                cancellationToken);
    }

    public async Task<IEnumerable<Medicine>> SearchAsync(
        string? search = null,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Set<Medicine>().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim().ToLower();
            query = query.Where(medicine =>
                medicine.Code.ToLower().Contains(normalizedSearch)
                || medicine.Name.ToLower().Contains(normalizedSearch)
                || medicine.UnitType.ToLower().Contains(normalizedSearch));
        }

        return await query
            .OrderBy(medicine => medicine.Code)
            .ThenBy(medicine => medicine.Name)
            .ThenBy(medicine => medicine.UnitQuantity)
            .ToListAsync(cancellationToken);
    }
}
