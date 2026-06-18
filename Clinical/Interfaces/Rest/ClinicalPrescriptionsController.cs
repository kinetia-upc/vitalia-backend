using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;
using VitaliaBackend.Clinical.Application.CommandServices;
using VitaliaBackend.Clinical.Application.QueryServices;
using VitaliaBackend.Clinical.Domain.Model;
using VitaliaBackend.Clinical.Domain.Model.Queries;
using VitaliaBackend.Clinical.Interfaces.Rest.Resources;
using VitaliaBackend.Clinical.Interfaces.Rest.Transform;
using VitaliaBackend.Resources.Errors;
using VitaliaBackend.Shared.Interfaces.Rest.ProblemDetails;

namespace VitaliaBackend.Clinical.Interfaces.Rest;

[ApiController]
[Route("api/v1/prescriptions")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Clinical prescriptions endpoints")]
public class ClinicalPrescriptionsController(
    IPrescriptionQueryService prescriptionQueryService,
    IPrescriptionCommandService prescriptionCommandService,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory) : ControllerBase
{
    [HttpGet("{prescriptionId:int}")]
    public async Task<IActionResult> GetPrescriptionById(
        [FromRoute] int prescriptionId,
        CancellationToken cancellationToken)
    {
        var query = new GetPrescriptionByIdQuery(prescriptionId);
        var prescription = await prescriptionQueryService.Handle(query, cancellationToken);

        return ClinicalActionResultAssembler.ToActionResultFromNullable(
            this,
            prescription,
            ClinicalError.PrescriptionNotFound,
            errorLocalizer,
            problemDetailsFactory,
            foundPrescription => Ok(PrescriptionResourceFromEntityAssembler.ToResourceFromEntity(foundPrescription)));
    }

    [HttpPost]
    public async Task<IActionResult> CreatePrescription(
        [FromBody] CreatePrescriptionResource resource,
        CancellationToken cancellationToken)
    {
        var command = CreatePrescriptionCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await prescriptionCommandService.Handle(command, cancellationToken);

        return ClinicalActionResultAssembler.ToActionResultFromResult(
            this,
            result,
            errorLocalizer,
            problemDetailsFactory,
            createdPrescription => CreatedAtAction(
                nameof(GetPrescriptionById),
                new { prescriptionId = createdPrescription.Id },
                PrescriptionResourceFromEntityAssembler.ToResourceFromEntity(createdPrescription)));
    }
}
