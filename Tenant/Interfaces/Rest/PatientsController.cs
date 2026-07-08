using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using VitaliaBackend.Tenant.Domain.Model.Aggregates;

namespace VitaliaBackend.Tenant.Interfaces.Rest;

[ApiController]
[Route("api/v1/patients")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Patients endpoints")]
public class PatientsController(AppDbContext context) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "List patients", Description = "Returns every patient profile registered in the platform.")]
    [SwaggerResponse(200, "The list of patients was returned.", typeof(IEnumerable<Patient>))]
    public async Task<IActionResult> GetPatients(CancellationToken cancellationToken)
    {
        return Ok(await context.Patients.AsNoTracking().ToListAsync(cancellationToken));
    }

    [HttpGet("{userId:guid}")]
    [SwaggerOperation(Summary = "Get a patient by user id", Description = "Returns a patient profile linked to an IAM user UUID.")]
    [SwaggerResponse(200, "The patient was found.", typeof(Patient))]
    [SwaggerResponse(404, "No patient exists for the given user id.")]
    public async Task<IActionResult> GetPatientById(
        [FromRoute, SwaggerParameter("UUID of the user linked to the patient.")]
        Guid userId,
        CancellationToken cancellationToken)
    {
        var patient = await context.Patients.AsNoTracking().FirstOrDefaultAsync(item => item.UserId == userId, cancellationToken);
        return patient is null ? NotFound() : Ok(patient);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a patient", Description = "Creates a patient profile for an existing IAM user.")]
    [SwaggerResponse(201, "The patient was created.", typeof(Patient))]
    public async Task<IActionResult> CreatePatient(
        [FromBody, SwaggerParameter("Data for the new patient profile.")]
        Patient resource,
        CancellationToken cancellationToken)
    {
        var patient = new Patient(resource.UserId, resource.Code, resource.InsuranceProvider, resource.PolicyNumber, resource.ActiveThru, resource.EmergencyContactName, resource.EmergencyContactPhone, resource.EHRCode);
        context.Patients.Add(patient);
        await context.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetPatientById), new { userId = patient.UserId }, patient);
    }

    [HttpPut("{userId:guid}")]
    [SwaggerOperation(Summary = "Update a patient", Description = "Updates insurance and emergency contact data for an existing patient profile.")]
    [SwaggerResponse(200, "The patient was updated.", typeof(Patient))]
    [SwaggerResponse(404, "No patient exists for the given user id.")]
    public async Task<IActionResult> UpdatePatient(
        [FromRoute, SwaggerParameter("UUID of the user linked to the patient.")]
        Guid userId,
        [FromBody, SwaggerParameter("New data for the patient profile.")]
        Patient resource,
        CancellationToken cancellationToken)
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
