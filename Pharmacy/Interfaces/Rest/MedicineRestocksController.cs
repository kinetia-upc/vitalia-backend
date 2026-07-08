using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using VitaliaBackend.Pharmacy.Domain.Model.Aggregates;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

namespace VitaliaBackend.Pharmacy.Interfaces.Rest;

public record CreateMedicineRestockResource(string Code, string BranchId, Guid MedicineId, int Quantity, Guid CreatedByUserId);

[ApiController]
[Route("api/v1/medicineRestocks")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Medicine restocks endpoints")]
public class MedicineRestocksController(AppDbContext context) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "List medicine restocks", Description = "Returns medicine restock records, optionally filtered by branch.")]
    [SwaggerResponse(200, "The list of medicine restocks was returned.", typeof(IEnumerable<MedicineRestock>))]
    public async Task<IActionResult> GetMedicineRestocks(
        [FromQuery, SwaggerParameter("Optional branch business id used to filter restocks.")]
        string? branchId,
        CancellationToken cancellationToken)
    {
        var query = context.MedicineRestocks.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(branchId))
            query = query.Where(item => item.BranchId == branchId);
        return Ok(await query.ToListAsync(cancellationToken));
    }

    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Get a medicine restock by id", Description = "Returns a single medicine restock identified by its UUID.")]
    [SwaggerResponse(200, "The medicine restock was found.", typeof(MedicineRestock))]
    [SwaggerResponse(404, "No medicine restock exists with the given id.")]
    public async Task<IActionResult> GetMedicineRestockById(
        [FromRoute, SwaggerParameter("UUID of the medicine restock.")]
        Guid id,
        CancellationToken cancellationToken)
    {
        var restock = await context.MedicineRestocks.AsNoTracking().FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        return restock is null ? NotFound() : Ok(restock);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a medicine restock", Description = "Registers a medicine restock and increases branch inventory stock.")]
    [SwaggerResponse(201, "The medicine restock was created.", typeof(MedicineRestock))]
    [SwaggerResponse(400, "The quantity must be greater than zero.")]
    [SwaggerResponse(404, "No branch medicine inventory entry exists for the given ids.")]
    public async Task<IActionResult> CreateMedicineRestock(
        [FromBody, SwaggerParameter("Data for the new medicine restock.")]
        CreateMedicineRestockResource resource,
        CancellationToken cancellationToken)
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
