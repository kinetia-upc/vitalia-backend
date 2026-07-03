using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitaliaBackend.Pharmacy.Domain.Model.Aggregates;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

namespace VitaliaBackend.Pharmacy.Interfaces.Rest;

public record BranchMedicineResource(string BranchId, Guid MedicineId, int Stock, decimal Price);

[ApiController]
[Route("api/v1/branchMedicines")]
[Produces(MediaTypeNames.Application.Json)]
public class BranchMedicinesController(AppDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetBranchMedicines([FromQuery] string? branchId, CancellationToken cancellationToken)
    {
        var query = context.BranchMedicines.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(branchId))
            query = query.Where(item => item.BranchId == branchId);
        return Ok(await query.ToListAsync(cancellationToken));
    }

    [HttpGet("branches/{branchId}/medicines/{medicineId:guid}")]
    public async Task<IActionResult> GetBranchMedicine([FromRoute] string branchId, [FromRoute] Guid medicineId, CancellationToken cancellationToken)
    {
        var branchMedicine = await context.BranchMedicines.AsNoTracking()
            .FirstOrDefaultAsync(item => item.BranchId == branchId && item.MedicineId == medicineId, cancellationToken);
        return branchMedicine is null ? NotFound() : Ok(branchMedicine);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBranchMedicine([FromBody] BranchMedicineResource resource, CancellationToken cancellationToken)
    {
        var exists = await context.BranchMedicines.AnyAsync(item => item.BranchId == resource.BranchId && item.MedicineId == resource.MedicineId, cancellationToken);
        if (exists) return Conflict(new { message = "Branch medicine already exists." });

        var branchMedicine = new BranchMedicine(resource.BranchId, resource.MedicineId, resource.Stock, resource.Price);
        context.BranchMedicines.Add(branchMedicine);
        await context.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetBranchMedicine), new { branchId = branchMedicine.BranchId, medicineId = branchMedicine.MedicineId }, branchMedicine);
    }

    [HttpPut("branches/{branchId}/medicines/{medicineId:guid}")]
    public async Task<IActionResult> UpdateBranchMedicine(
        [FromRoute] string branchId, [FromRoute] Guid medicineId,
        [FromBody] BranchMedicineResource resource, CancellationToken cancellationToken)
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
    public async Task<IActionResult> DeleteBranchMedicine(
        [FromRoute] string branchId, [FromRoute] Guid medicineId, CancellationToken cancellationToken)
    {
        var branchMedicine = await context.BranchMedicines
            .FirstOrDefaultAsync(item => item.BranchId == branchId && item.MedicineId == medicineId, cancellationToken);
        if (branchMedicine is null) return NotFound();
        context.BranchMedicines.Remove(branchMedicine);
        await context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}
