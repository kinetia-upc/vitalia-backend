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
[Route("api/v1/prescription-details")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Clinical prescription details endpoints")]
public class ClinicalPrescriptionDetailsController(
    IPrescriptionDetailQueryService prescriptionDetailQueryService,
    IPrescriptionDetailCommandService prescriptionDetailCommandService) : ControllerBase
{
    [HttpGet("{prescriptionDetailId:int}")]
    public async Task<IActionResult> GetPrescriptionDetailById(
        [FromRoute] int prescriptionDetailId,
        CancellationToken cancellationToken)
    {
        var query = new GetPrescriptionDetailByIdQuery(prescriptionDetailId);
        var prescriptionDetail = await prescriptionDetailQueryService.Handle(query, cancellationToken);

        if (prescriptionDetail is null)
            return NotFound(ClinicalErrors.PrescriptionDetailNotFoundError);

        return Ok(PrescriptionDetailResourceFromEntityAssembler.ToResourceFromEntity(prescriptionDetail));
    }

    [HttpPost]
    public async Task<IActionResult> CreatePrescriptionDetail(
        [FromBody] CreatePrescriptionDetailResource resource,
        CancellationToken cancellationToken)
    {
        var command = CreatePrescriptionDetailCommandFromResourceAssembler.ToCommandFromResource(resource);
        var prescriptionDetail = await prescriptionDetailCommandService.Handle(command, cancellationToken);

        if (prescriptionDetail is null)
            return BadRequest(ClinicalErrors.PrescriptionDetailCreationError);

        var prescriptionDetailResource =
            PrescriptionDetailResourceFromEntityAssembler.ToResourceFromEntity(prescriptionDetail);

        return CreatedAtAction(
            nameof(GetPrescriptionDetailById),
            new { prescriptionDetailId = prescriptionDetail.Id },
            prescriptionDetailResource);
    }
}
