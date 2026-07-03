using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitaliaBackend.Pharmacy.Domain.Model.Aggregates;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

namespace VitaliaBackend.Pharmacy.Interfaces.Rest;

public record CreateMedicineRestockResource(string Code, string BranchId, Guid MedicineId, int Quantity, Guid CreatedByUserId);

[ApiController]
[Route("api/v1/medicineRestocks")]
[Produces(MediaTypeNames.Application.Json)]
public class MedicineRestocksController(AppDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMedicineRestocks([FromQuery] string? branchId, CancellationToken cancellationToken)
    {
        var query = context.MedicineRestocks.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(branchId))
            query = query.Where(item => item.BranchId == branchId);
        return Ok(await query.ToListAsync(cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetMedicineRestockById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var restock = await context.MedicineRestocks.AsNoTracking().FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        return restock is null ? NotFound() : Ok(restock);
    }

    [HttpPost]
    public async Task<IActionResult> CreateMedicineRestock([FromBody] CreateMedicineRestockResource resource, CancellationToken cancellationToken)
    {
        if (resource.Quantity <= 0) return BadRequest(new { message = "Quantity must be greater than zero." });

        var branchMedicine = await context.BranchMedicines
            .FirstOrDefaultAsync(item => item.BranchId == resource.BranchId && item.MedicineId == resource.MedicineId, cancellationToken);
        if (branchMedicine is null) return NotFound(new { message = "Branch medicine was not found." });

        branchMedicine.IncreaseStock(resource.Quantity);
        var restock = new MedicineRestock(Guid.NewGuid(), resource.Code, resource.BranchId, resource.MedicineId, resource.Quantity, resource.CreatedByUserId);
        context.MedicineRestocks.Add(restock);
        await context.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetMedicineRestockById), new { id = restock.Id }, restock);
    }
}
