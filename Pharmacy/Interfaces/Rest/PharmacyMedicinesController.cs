using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;
using VitaliaBackend.Pharmacy.Application.CommandServices;
using VitaliaBackend.Pharmacy.Application.QueryServices;
using VitaliaBackend.Pharmacy.Domain.Model;
using VitaliaBackend.Pharmacy.Domain.Model.Commands;
using VitaliaBackend.Pharmacy.Domain.Model.Queries;
using VitaliaBackend.Pharmacy.Interfaces.Rest.Resources;
using VitaliaBackend.Pharmacy.Interfaces.Rest.Transform;
using VitaliaBackend.Resources.Errors;
using VitaliaBackend.Shared.Interfaces.Rest.ProblemDetails;

namespace VitaliaBackend.Pharmacy.Interfaces.Rest;

[ApiController]
[Route("api/v1/medicines")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Pharmacy medicines endpoints")]
public class PharmacyMedicinesController(
    IMedicineQueryService medicineQueryService,
    IMedicineCommandService medicineCommandService,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory) : ControllerBase
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

        return PharmacyActionResultAssembler.ToActionResultFromNullable(
            this,
            medicine,
            PharmacyError.MedicineNotFoundError,
            errorLocalizer,
            problemDetailsFactory,
            foundMedicine => Ok(MedicineResourceFromEntityAssembler.ToResourceFromEntity(foundMedicine)));
    }

    [HttpPost]
    public async Task<IActionResult> CreateMedicine(
        [FromBody] CreateMedicineResource resource,
        CancellationToken cancellationToken)
    {
        var command = CreateMedicineCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await medicineCommandService.Handle(command, cancellationToken);

        return PharmacyActionResultAssembler.ToActionResultFromResult(
            this,
            result,
            errorLocalizer,
            problemDetailsFactory,
            createdMedicine => CreatedAtAction(
                nameof(GetMedicineById),
                new { medicineId = createdMedicine.Id },
                MedicineResourceFromEntityAssembler.ToResourceFromEntity(createdMedicine)));
    }

    [HttpPut("{medicineId:int}")]
    public async Task<IActionResult> UpdateMedicine(
        [FromRoute] int medicineId,
        [FromBody] UpdateMedicineResource resource,
        CancellationToken cancellationToken)
    {
        var command = UpdateMedicineCommandFromResourceAssembler.ToCommandFromResource(medicineId, resource);
        var result = await medicineCommandService.Handle(command, cancellationToken);

        return PharmacyActionResultAssembler.ToActionResultFromResult(
            this,
            result,
            errorLocalizer,
            problemDetailsFactory,
            updatedMedicine => Ok(MedicineResourceFromEntityAssembler.ToResourceFromEntity(updatedMedicine)));
    }

    [HttpDelete("{medicineId:int}")]
    public async Task<IActionResult> DeleteMedicine(
        [FromRoute] int medicineId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteMedicineCommand(medicineId);
        var result = await medicineCommandService.Handle(command, cancellationToken);

        return PharmacyActionResultAssembler.ToActionResultFromResult(
            this,
            result,
            errorLocalizer,
            problemDetailsFactory,
            NoContent);
    }
}
