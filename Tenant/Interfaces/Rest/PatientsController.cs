using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using VitaliaBackend.Tenant.Domain.Model.Aggregates;

namespace VitaliaBackend.Tenant.Interfaces.Rest;

[ApiController]
[Route("api/v1/patients")]
[Produces(MediaTypeNames.Application.Json)]
public class PatientsController(AppDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetPatients(CancellationToken cancellationToken)
    {
        return Ok(await context.Patients.AsNoTracking().ToListAsync(cancellationToken));
    }

    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetPatientById([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var patient = await context.Patients.AsNoTracking().FirstOrDefaultAsync(item => item.UserId == userId, cancellationToken);
        return patient is null ? NotFound() : Ok(patient);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePatient([FromBody] Patient resource, CancellationToken cancellationToken)
    {
        var patient = new Patient(resource.UserId, resource.Code, resource.InsuranceProvider, resource.PolicyNumber, resource.ActiveThru, resource.EmergencyContactName, resource.EmergencyContactPhone, resource.EHRCode);
        context.Patients.Add(patient);
        await context.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetPatientById), new { userId = patient.UserId }, patient);
    }

    [HttpPut("{userId:guid}")]
    public async Task<IActionResult> UpdatePatient([FromRoute] Guid userId, [FromBody] Patient resource, CancellationToken cancellationToken)
    {
        var patient = await context.Patients.FirstOrDefaultAsync(item => item.UserId == userId, cancellationToken);
        if (patient is null) return NotFound();

        patient.UpdateDetails(
            resource.InsuranceProvider,
            resource.PolicyNumber,
            resource.ActiveThru,
            resource.EmergencyContactName,
            resource.EmergencyContactPhone);

        await context.SaveChangesAsync(cancellationToken);
        return Ok(patient);
    }
}
