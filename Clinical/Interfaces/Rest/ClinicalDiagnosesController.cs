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
[Route("api/v1/diagnoses")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Clinical diagnoses endpoints")]
public class ClinicalDiagnosesController(
    IDiagnosisQueryService diagnosisQueryService,
    IDiagnosisCommandService diagnosisCommandService) : ControllerBase
{
    [HttpGet("{diagnosisId:int}")]
    public async Task<IActionResult> GetDiagnosisById(
        [FromRoute] int diagnosisId,
        CancellationToken cancellationToken)
    {
        var query = new GetDiagnosisByIdQuery(diagnosisId);
        var diagnosis = await diagnosisQueryService.Handle(query, cancellationToken);

        if (diagnosis is null)
            return NotFound(ClinicalErrors.DiagnosisNotFoundError);

        return Ok(DiagnosisResourceFromEntityAssembler.ToResourceFromEntity(diagnosis));
    }

    [HttpPost]
    public async Task<IActionResult> CreateDiagnosis(
        [FromBody] CreateDiagnosisResource resource,
        CancellationToken cancellationToken)
    {
        var command = CreateDiagnosisCommandFromResourceAssembler.ToCommandFromResource(resource);
        var diagnosis = await diagnosisCommandService.Handle(command, cancellationToken);

        if (diagnosis is null)
            return BadRequest(ClinicalErrors.DiagnosisCreationError);

        var diagnosisResource = DiagnosisResourceFromEntityAssembler.ToResourceFromEntity(diagnosis);

        return CreatedAtAction(
            nameof(GetDiagnosisById),
            new { diagnosisId = diagnosis.Id },
            diagnosisResource);
    }
}
