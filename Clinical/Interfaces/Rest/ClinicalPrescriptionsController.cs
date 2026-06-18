using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using VitaliaBackend.Clinical.Application.CommandServices;
using VitaliaBackend.Clinical.Application.QueryServices;
using VitaliaBackend.Clinical.Domain.Model.Errors;
using VitaliaBackend.Clinical.Domain.Model.Queries;
using VitaliaBackend.Clinical.Interfaces.Rest.Resources;
using VitaliaBackend.Clinical.Interfaces.Rest.Transform;

namespace VitaliaBackend.Clinical.Interfaces.Rest;

[ApiController]
[Route("api/v1/prescriptions")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Clinical prescriptions endpoints")]
public class ClinicalPrescriptionsController(
    IPrescriptionQueryService prescriptionQueryService,
    IPrescriptionCommandService prescriptionCommandService) : ControllerBase
{
    [HttpGet("{prescriptionId:int}")]
    public async Task<IActionResult> GetPrescriptionById(
        [FromRoute] int prescriptionId,
        CancellationToken cancellationToken)
    {
        var query = new GetPrescriptionByIdQuery(prescriptionId);
        var prescription = await prescriptionQueryService.Handle(query, cancellationToken);

        if (prescription is null)
            return NotFound(ClinicalErrors.PrescriptionNotFoundError);

        return Ok(PrescriptionResourceFromEntityAssembler.ToResourceFromEntity(prescription));
    }

    [HttpPost]
    public async Task<IActionResult> CreatePrescription(
        [FromBody] CreatePrescriptionResource resource,
        CancellationToken cancellationToken)
    {
        var command = CreatePrescriptionCommandFromResourceAssembler.ToCommandFromResource(resource);
        var prescription = await prescriptionCommandService.Handle(command, cancellationToken);

        if (prescription is null)
            return BadRequest(ClinicalErrors.PrescriptionCreationError);

        var prescriptionResource = PrescriptionResourceFromEntityAssembler.ToResourceFromEntity(prescription);

        return CreatedAtAction(
            nameof(GetPrescriptionById),
            new { prescriptionId = prescription.Id },
            prescriptionResource);
    }
}
