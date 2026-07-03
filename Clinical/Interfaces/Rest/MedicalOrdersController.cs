using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

namespace VitaliaBackend.Clinical.Interfaces.Rest;

public record CreateMedicalOrderResource(string Code, Guid PatientId, Guid DoctorId, Guid AppointmentId, Guid? MedicalRecordId, string Type, string Description, string Status);
public record UpdateMedicalOrderResource(string Type, string Description, string Status);

[ApiController]
[Route("api/v1/medicalOrders")]
[Produces(MediaTypeNames.Application.Json)]
public class MedicalOrdersController(AppDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMedicalOrders([FromQuery] Guid? patientId, [FromQuery] Guid? appointmentId, CancellationToken cancellationToken)
    {
        var query = context.MedicalOrders.AsNoTracking();
        if (patientId.HasValue) query = query.Where(item => item.PatientId == patientId.Value);
        if (appointmentId.HasValue) query = query.Where(item => item.AppointmentId == appointmentId.Value);
        return Ok(await query.ToListAsync(cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetMedicalOrderById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var order = await context.MedicalOrders.AsNoTracking().FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpGet("byCode/{code}")]
    public async Task<IActionResult> GetMedicalOrderByCode([FromRoute] string code, CancellationToken cancellationToken)
    {
        var order = await context.MedicalOrders.AsNoTracking().FirstOrDefaultAsync(item => item.Code == code, cancellationToken);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpPost]
    public async Task<IActionResult> CreateMedicalOrder([FromBody] CreateMedicalOrderResource resource, CancellationToken cancellationToken)
    {
        var order = new MedicalOrder(Guid.NewGuid(), resource.Code, resource.PatientId, resource.DoctorId, resource.AppointmentId, resource.MedicalRecordId, resource.Type, resource.Description, resource.Status);
        context.MedicalOrders.Add(order);
        await context.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetMedicalOrderById), new { id = order.Id }, order);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> UpdateMedicalOrder([FromRoute] Guid id, [FromBody] UpdateMedicalOrderResource resource, CancellationToken cancellationToken)
    {
        var order = await context.MedicalOrders.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (order is null) return NotFound();
        order.Update(resource.Type, resource.Description, resource.Status);
        await context.SaveChangesAsync(cancellationToken);
        return Ok(order);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteMedicalOrder([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var order = await context.MedicalOrders.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (order is null) return NotFound();
        context.MedicalOrders.Remove(order);
        await context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}
