using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using VitaliaBackend.Pharmacy.Application.CommandServices;
using VitaliaBackend.Pharmacy.Application.QueryServices;
using VitaliaBackend.Pharmacy.Domain.Model.Commands;
using VitaliaBackend.Pharmacy.Domain.Model.Errors;
using VitaliaBackend.Pharmacy.Domain.Model.Queries;
using VitaliaBackend.Pharmacy.Interfaces.Rest.Resources;
using VitaliaBackend.Pharmacy.Interfaces.Rest.Transform;

namespace VitaliaBackend.Pharmacy.Interfaces.Rest;

[ApiController]
[Route("api/v1/medicines")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Pharmacy medicines endpoints")]
public class PharmacyMedicinesController(
    IMedicineQueryService medicineQueryService,
    IMedicineCommandService medicineCommandService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMedicines(
        [FromQuery] string? search,
        CancellationToken cancellationToken)
    {
        var query = new GetMedicinesQuery(search);
        var medicines = await medicineQueryService.Handle(query, cancellationToken);
        var resources = medicines.Select(MedicineResourceFromEntityAssembler.ToResourceFromEntity);

        return Ok(resources);
    }

    [HttpGet("{medicineId:int}")]
    public async Task<IActionResult> GetMedicineById(
        [FromRoute] int medicineId,
        CancellationToken cancellationToken)
    {
        var query = new GetMedicineByIdQuery(medicineId);
        var medicine = await medicineQueryService.Handle(query, cancellationToken);

        if (medicine is null)
            return NotFound(PharmacyErrors.MedicineNotFoundError);

        return Ok(MedicineResourceFromEntityAssembler.ToResourceFromEntity(medicine));
    }

    [HttpPost]
    public async Task<IActionResult> CreateMedicine(
        [FromBody] CreateMedicineResource resource,
        CancellationToken cancellationToken)
    {
        var command = CreateMedicineCommandFromResourceAssembler.ToCommandFromResource(resource);
        var medicine = await medicineCommandService.Handle(command, cancellationToken);

        if (medicine is null)
            return BadRequest(PharmacyErrors.MedicineCreationError);

        var medicineResource = MedicineResourceFromEntityAssembler.ToResourceFromEntity(medicine);

        return CreatedAtAction(
            nameof(GetMedicineById),
            new { medicineId = medicine.Id },
            medicineResource);
    }

    [HttpPut("{medicineId:int}")]
    public async Task<IActionResult> UpdateMedicine(
        [FromRoute] int medicineId,
        [FromBody] UpdateMedicineResource resource,
        CancellationToken cancellationToken)
    {
        var command = UpdateMedicineCommandFromResourceAssembler.ToCommandFromResource(medicineId, resource);
        var medicine = await medicineCommandService.Handle(command, cancellationToken);

        if (medicine is null)
            return BadRequest(PharmacyErrors.MedicineUpdateError);

        return Ok(MedicineResourceFromEntityAssembler.ToResourceFromEntity(medicine));
    }

    [HttpDelete("{medicineId:int}")]
    public async Task<IActionResult> DeleteMedicine(
        [FromRoute] int medicineId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteMedicineCommand(medicineId);
        var deleted = await medicineCommandService.Handle(command, cancellationToken);

        if (!deleted)
            return NotFound(PharmacyErrors.MedicineNotFoundError);

        return NoContent();
    }
}
