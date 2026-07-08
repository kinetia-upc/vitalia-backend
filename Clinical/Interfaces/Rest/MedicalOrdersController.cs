using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

namespace VitaliaBackend.Clinical.Interfaces.Rest;

public record CreateMedicalOrderResource(string Code, Guid PatientId, Guid DoctorId, Guid AppointmentId, Guid? MedicalRecordId, string Type, string Description, string Status);
public record UpdateMedicalOrderResource(string Type, string Description, string Status);

[ApiController]
[Route("api/v1/medicalOrders")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Medical orders endpoints")]
public class MedicalOrdersController(AppDbContext context) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "List medical orders", Description = "Returns medical orders, optionally filtered by patient or appointment.")]
    [SwaggerResponse(200, "The list of medical orders was returned.", typeof(IEnumerable<MedicalOrder>))]
    public async Task<IActionResult> GetMedicalOrders(
        [FromQuery, SwaggerParameter("Optional UUID of the patient used to filter orders.")]
        Guid? patientId,
        [FromQuery, SwaggerParameter("Optional UUID of the appointment used to filter orders.")]
        Guid? appointmentId,
        CancellationToken cancellationToken)
    {
        var query = context.MedicalOrders.AsNoTracking();
        if (patientId.HasValue) query = query.Where(item => item.PatientId == patientId.Value);
        if (appointmentId.HasValue) query = query.Where(item => item.AppointmentId == appointmentId.Value);
        return Ok(await query.ToListAsync(cancellationToken));
    }

    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Get a medical order by id", Description = "Returns a single medical order identified by its UUID.")]
    [SwaggerResponse(200, "The medical order was found.", typeof(MedicalOrder))]
    [SwaggerResponse(404, "No medical order exists with the given id.")]
    public async Task<IActionResult> GetMedicalOrderById(
        [FromRoute, SwaggerParameter("UUID of the medical order.")]
        Guid id,
        CancellationToken cancellationToken)
    {
        var order = await context.MedicalOrders.AsNoTracking().FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpGet("byCode/{code}")]
    [SwaggerOperation(Summary = "Get a medical order by code", Description = "Returns a single medical order identified by its business code.")]
    [SwaggerResponse(200, "The medical order was found.", typeof(MedicalOrder))]
    [SwaggerResponse(404, "No medical order exists with the given code.")]
    public async Task<IActionResult> GetMedicalOrderByCode(
        [FromRoute, SwaggerParameter("Business code of the medical order.")]
        string code,
        CancellationToken cancellationToken)
    {
        var order = await context.MedicalOrders.AsNoTracking().FirstOrDefaultAsync(item => item.Code == code, cancellationToken);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a medical order", Description = "Creates a medical order for a patient, doctor and appointment.")]
    [SwaggerResponse(201, "The medical order was created.", typeof(MedicalOrder))]
    public async Task<IActionResult> CreateMedicalOrder(
        [FromBody, SwaggerParameter("Data for the new medical order.")]
        CreateMedicalOrderResource resource,
        CancellationToken cancellationToken)
    {
        var order = new MedicalOrder(Guid.NewGuid(), resource.Code, resource.PatientId, resource.DoctorId, resource.AppointmentId, resource.MedicalRecordId, resource.Type, resource.Description, resource.Status);
        context.MedicalOrders.Add(order);
        await context.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetMedicalOrderById), new { id = order.Id }, order);
    }

    [HttpPatch("{id:guid}")]
    [SwaggerOperation(Summary = "Update a medical order", Description = "Updates type, description and status for an existing medical order.")]
    [SwaggerResponse(200, "The medical order was updated.", typeof(MedicalOrder))]
    [SwaggerResponse(404, "No medical order exists with the given id.")]
    public async Task<IActionResult> UpdateMedicalOrder(
        [FromRoute, SwaggerParameter("UUID of the medical order to update.")]
        Guid id,
        [FromBody, SwaggerParameter("New data for the medical order.")]
        UpdateMedicalOrderResource resource,
        CancellationToken cancellationToken)
    {
        var order = await context.MedicalOrders.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (order is null) return NotFound();
        order.Update(resource.Type, resource.Description, resource.Status);
        await context.SaveChangesAsync(cancellationToken);
        return Ok(order);
    }

    [HttpDelete("{id:guid}")]
    [SwaggerOperation(Summary = "Delete a medical order", Description = "Permanently removes a medical order by its UUID.")]
    [SwaggerResponse(204, "The medical order was deleted.")]
    [SwaggerResponse(404, "No medical order exists with the given id.")]
    public async Task<IActionResult> DeleteMedicalOrder(
        [FromRoute, SwaggerParameter("UUID of the medical order to delete.")]
        Guid id,
        CancellationToken cancellationToken)
    {
        var order = await context.MedicalOrders.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (order is null) return NotFound();
        context.MedicalOrders.Remove(order);
        await context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}
