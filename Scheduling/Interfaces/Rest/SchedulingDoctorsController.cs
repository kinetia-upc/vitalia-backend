using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using VitaliaBackend.Scheduling.Application.QueryServices;
using VitaliaBackend.Scheduling.Domain.Model.Queries;
using VitaliaBackend.Scheduling.Interfaces.Rest.Transform;

namespace VitaliaBackend.Scheduling.Interfaces.Rest;

[ApiController]
[Route("api/v1/scheduling/doctors")]
[Produces(MediaTypeNames.Application.Json)]
public class SchedulingDoctorsController(ISchedulingDoctorQueryService doctorQueryService)
    : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllDoctors(CancellationToken cancellationToken)
    {
        var doctors = await doctorQueryService.Handle(new GetAllSchedulingDoctorsQuery(), cancellationToken);
        return Ok(doctors.Select(SchedulingDoctorResourceFromEntityAssembler.ToResourceFromEntity));
    }
}