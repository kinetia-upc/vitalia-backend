namespace VitaliaBackend.Tenant.Domain.Model.Queries;

/// <summary>
///     Asks for the list of branches, optionally filtered to a single healthcare center.
/// </summary>
public record GetBranchesQuery(string? HealthcareCenterId = null);
