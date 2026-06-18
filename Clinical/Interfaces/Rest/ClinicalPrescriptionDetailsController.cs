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
[Route("api/v1/prescription-details")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Clinical prescription details endpoints")]
public class ClinicalPrescriptionDetailsController(
    IPrescriptionDetailQueryService prescriptionDetailQueryService,
    IPrescriptionDetailCommandService prescriptionDetailCommandService,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory) : ControllerBase
{
    [HttpGet("{prescriptionDetailId:int}")]
    public async Task<IActionResult> GetPrescriptionDetailById(
        [FromRoute] int prescriptionDetailId,
        CancellationToken cancellationToken)
    {
        var query = new GetPrescriptionDetailByIdQuery(prescriptionDetailId);
        var prescriptionDetail = await prescriptionDetailQueryService.Handle(query, cancellationToken);

        return ClinicalActionResultAssembler.ToActionResultFromNullable(
            this,
            prescriptionDetail,
            ClinicalError.PrescriptionDetailNotFound,
            errorLocalizer,
            problemDetailsFactory,
            foundPrescriptionDetail => Ok(
                PrescriptionDetailResourceFromEntityAssembler.ToResourceFromEntity(foundPrescriptionDetail)));
    }

    [HttpPost]
    public async Task<IActionResult> CreatePrescriptionDetail(
        [FromBody] CreatePrescriptionDetailResource resource,
        CancellationToken cancellationToken)
    {
        var command = CreatePrescriptionDetailCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await prescriptionDetailCommandService.Handle(command, cancellationToken);

        return ClinicalActionResultAssembler.ToActionResultFromResult(
            this,
            result,
            errorLocalizer,
            problemDetailsFactory,
            createdPrescriptionDetail => CreatedAtAction(
                nameof(GetPrescriptionDetailById),
                new { prescriptionDetailId = createdPrescriptionDetail.Id },
                PrescriptionDetailResourceFromEntityAssembler.ToResourceFromEntity(createdPrescriptionDetail)));
    }
}
