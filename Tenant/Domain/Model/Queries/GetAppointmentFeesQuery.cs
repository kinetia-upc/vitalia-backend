namespace VitaliaBackend.Tenant.Domain.Model.Queries;

/// <summary>
///     Asks for the list of appointment fees, optionally filtered to a single branch.
/// </summary>
public record GetAppointmentFeesQuery(string? BranchId = null);
