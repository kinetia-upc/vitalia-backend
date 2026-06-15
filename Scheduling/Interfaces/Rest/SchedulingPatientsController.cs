using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using VitaliaBackend.Scheduling.Application.QueryServices;
using VitaliaBackend.Scheduling.Domain.Model.Queries;
using VitaliaBackend.Scheduling.Interfaces.Rest.Transform;

namespace VitaliaBackend.Scheduling.Interfaces.Rest;

[ApiController]
[Route("api/v1/scheduling/patients")]
[Produces(MediaTypeNames.Application.Json)]
public class SchedulingPatientsController(ISchedulingPatientQueryService patientQueryService)
    : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllPatients(CancellationToken cancellationToken)
    {
        var patients = await patientQueryService.Handle(new GetAllSchedulingPatientsQuery(), cancellationToken);
        return Ok(patients.Select(SchedulingPatientResourceFromEntityAssembler.ToResourceFromEntity));
    }
}