using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using VitaliaBackend.Pharmacy.Domain.Model.Aggregates;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

namespace VitaliaBackend.Pharmacy.Interfaces.Rest;

public record BranchMedicineResource(string BranchId, Guid MedicineId, int Stock, decimal Price);

[ApiController]
[Route("api/v1/branchMedicines")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Branch medicines endpoints")]
public class BranchMedicinesController(AppDbContext context) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "List branch medicines", Description = "Returns branch medicine inventory entries, optionally filtered by branch.")]
    [SwaggerResponse(200, "The list of branch medicines was returned.", typeof(IEnumerable<BranchMedicine>))]
    public async Task<IActionResult> GetBranchMedicines(
        [FromQuery, SwaggerParameter("Optional branch business id used to filter inventory.")]
        string? branchId,
        CancellationToken cancellationToken)
    {
        var query = context.BranchMedicines.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(branchId))
            query = query.Where(item => item.BranchId == branchId);
        return Ok(await query.ToListAsync(cancellationToken));
    }

    [HttpGet("branches/{branchId}/medicines/{medicineId:guid}")]
    [SwaggerOperation(Summary = "Get a branch medicine", Description = "Returns one inventory entry by branch id and medicine UUID.")]
    [SwaggerResponse(200, "The branch medicine was found.", typeof(BranchMedicine))]
    [SwaggerResponse(404, "No branch medicine exists for the given ids.")]
    public async Task<IActionResult> GetBranchMedicine(
        [FromRoute, SwaggerParameter("Business id of the branch.")]
        string branchId,
        [FromRoute, SwaggerParameter("UUID of the medicine.")]
        Guid medicineId,
        CancellationToken cancellationToken)
    {
        var branchMedicine = await context.BranchMedicines.AsNoTracking()
            .FirstOrDefaultAsync(item => item.BranchId == branchId && item.MedicineId == medicineId, cancellationToken);
        return branchMedicine is null ? NotFound() : Ok(branchMedicine);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a branch medicine", Description = "Creates an inventory entry for a medicine in a branch.")]
    [SwaggerResponse(201, "The branch medicine was created.", typeof(BranchMedicine))]
    [SwaggerResponse(409, "The branch already has an inventory entry for this medicine.")]
    public async Task<IActionResult> CreateBranchMedicine(
        [FromBody, SwaggerParameter("Data for the new branch medicine inventory entry.")]
        BranchMedicineResource resource,
        CancellationToken cancellationToken)
    {
        var exists = await context.BranchMedicines.AnyAsync(item => item.BranchId == resource.BranchId && item.MedicineId == resource.MedicineId, cancellationToken);
        if (exists) return Conflict(new { message = "Branch medicine already exists." });

        var branchMedicine = new BranchMedicine(resource.BranchId, resource.MedicineId, resource.Stock, resource.Price);
        context.BranchMedicines.Add(branchMedicine);
        await context.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetBranchMedicine), new { branchId = branchMedicine.BranchId, medicineId = branchMedicine.MedicineId }, branchMedicine);
    }

    [HttpPut("branches/{branchId}/medicines/{medicineId:guid}")]
    [SwaggerOperation(Summary = "Update a branch medicine", Description = "Updates stock and price for a branch medicine inventory entry.")]
    [SwaggerResponse(200, "The branch medicine was updated.", typeof(BranchMedicineResource))]
    [SwaggerResponse(404, "No branch medicine exists for the given ids.")]
    [SwaggerResponse(409, "The target branch already has this medicine assigned.")]
    public async Task<IActionResult> UpdateBranchMedicine(
        [FromRoute, SwaggerParameter("Business id of the branch.")]
        string branchId,
        [FromRoute, SwaggerParameter("UUID of the medicine.")]
        Guid medicineId,
        [FromBody, SwaggerParameter("New data for the branch medicine inventory entry.")]
        BranchMedicineResource resource,
        CancellationToken cancellationToken)
    {
        var branchMedicine = await context.BranchMedicines
            .FirstOrDefaultAsync(item => item.BranchId == branchId && item.MedicineId == medicineId, cancellationToken);
        if (branchMedicine is null) return NotFound();

        if (resource.BranchId != branchId)
        {
            var targetExists = await context.BranchMedicines
                .AnyAsync(item => item.BranchId == resource.BranchId && item.MedicineId == resource.MedicineId, cancellationToken);
            if (targetExists) return Conflict(new { message = "Branch medicine already exists for the target branch." });

            context.BranchMedicines.Remove(branchMedicine);
            var moved = new BranchMedicine(resource.BranchId, resource.MedicineId, resource.Stock, resource.Price);
            context.BranchMedicines.Add(moved);
        }
        else
        {
            branchMedicine.UpdateDetails(resource.Stock, resource.Price);
        }

        await context.SaveChangesAsync(cancellationToken);
        return Ok(new BranchMedicineResource(branchId, medicineId, branchMedicine.Stock, branchMedicine.Price));
    }

    [HttpDelete("branches/{branchId}/medicines/{medicineId:guid}")]
    [SwaggerOperation(Summary = "Delete a branch medicine", Description = "Permanently removes a medicine inventory entry from a branch.")]
    [SwaggerResponse(204, "The branch medicine was deleted.")]
    [SwaggerResponse(404, "No branch medicine exists for the given ids.")]
    public async Task<IActionResult> DeleteBranchMedicine(
        [FromRoute, SwaggerParameter("Business id of the branch.")]
        string branchId,
        [FromRoute, SwaggerParameter("UUID of the medicine.")]
        Guid medicineId,
        CancellationToken cancellationToken)
    {
        var branchMedicine = await context.BranchMedicines
            .FirstOrDefaultAsync(item => item.BranchId == branchId && item.MedicineId == medicineId, cancellationToken);
        if (branchMedicine is null) return NotFound();
        context.BranchMedicines.Remove(branchMedicine);
        await context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}
