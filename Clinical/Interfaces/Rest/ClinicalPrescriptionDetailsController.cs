using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;
using VitaliaBackend.Clinical.Application.CommandServices;
using VitaliaBackend.Clinical.Application.QueryServices;
using VitaliaBackend.Clinical.Domain.Model;
using VitaliaBackend.Clinical.Domain.Model.Commands;
using VitaliaBackend.Clinical.Domain.Model.Queries;
using VitaliaBackend.Clinical.Interfaces.Rest.Resources;
using VitaliaBackend.Clinical.Interfaces.Rest.Transform;
using VitaliaBackend.Resources.Errors;
using VitaliaBackend.Shared.Interfaces.Rest.ProblemDetails;

namespace VitaliaBackend.Clinical.Interfaces.Rest;

[ApiController]
[Route("api/v1/prescriptionDetails")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Clinical prescription details endpoints")]
public class ClinicalPrescriptionDetailsController(
    IPrescriptionDetailQueryService prescriptionDetailQueryService,
    IPrescriptionDetailCommandService prescriptionDetailCommandService,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(
        Summary = "List all prescription details",
        Description = "Retrieves a list of all prescription details available in the system."
    )]
    public async Task<IActionResult> GetPrescriptionDetails(CancellationToken cancellationToken)
    {
        var query = new GetAllPrescriptionDetailsQuery();
        var prescriptionDetails = await prescriptionDetailQueryService.Handle(query, cancellationToken);
        var resources = prescriptionDetails.Select(PrescriptionDetailResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    [HttpGet("{prescriptionDetailId:int}")]
    [SwaggerOperation(
        Summary = "Get a prescription detail by id",
        Description = "Returns a single prescription detail using its numeric identifier."
    )]
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

    [HttpGet("prescriptions/{prescriptionId:int}")]
    [SwaggerOperation(
        Summary = "List prescription details by prescription",
        Description = "Returns all prescription details associated with the specified prescription identifier."
    )]
    public async Task<IActionResult> GetPrescriptionDetailsByPrescriptionId(
        [FromRoute] int prescriptionId,
        CancellationToken cancellationToken)
    {
        var query = new GetPrescriptionDetailsByPrescriptionIdQuery(prescriptionId);
        var prescriptionDetails = await prescriptionDetailQueryService.Handle(query, cancellationToken);
        var resources = prescriptionDetails.Select(PrescriptionDetailResourceFromEntityAssembler.ToResourceFromEntity);

        return Ok(resources);
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a prescription detail",
        Description = "Creates a new prescription detail entry for the specified prescription."
    )]
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

    [HttpPut("{prescriptionDetailId:int}")]
    [SwaggerOperation(
        Summary = "Update a prescription detail",
        Description = "Updates an existing prescription detail using its numeric identifier."
    )]
    public async Task<IActionResult> UpdatePrescriptionDetail(
        [FromRoute] int prescriptionDetailId,
        [FromBody] UpdatePrescriptionDetailResource resource,
        CancellationToken cancellationToken)
    {
        var command = UpdatePrescriptionDetailCommandFromResourceAssembler.ToCommandFromResource(
            prescriptionDetailId,
            resource);
        var result = await prescriptionDetailCommandService.Handle(command, cancellationToken);

        return ClinicalActionResultAssembler.ToActionResultFromResult(
            this,
            result,
            errorLocalizer,
            problemDetailsFactory,
            updatedPrescriptionDetail => Ok(
                PrescriptionDetailResourceFromEntityAssembler.ToResourceFromEntity(updatedPrescriptionDetail)));
    }

    [HttpDelete("{prescriptionDetailId:int}")]
    [SwaggerOperation(
        Summary = "Delete a prescription detail",
        Description = "Deletes an existing prescription detail using its numeric identifier."
    )]
    public async Task<IActionResult> DeletePrescriptionDetail(
        [FromRoute] int prescriptionDetailId,
        CancellationToken cancellationToken)
    {
        var command = new DeletePrescriptionDetailCommand(prescriptionDetailId);
        var result = await prescriptionDetailCommandService.Handle(command, cancellationToken);

        return ClinicalActionResultAssembler.ToActionResultFromResult(
            this,
            result,
            errorLocalizer,
            problemDetailsFactory,
            NoContent);
    }
}
