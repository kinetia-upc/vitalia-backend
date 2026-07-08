using VitaliaBackend.Shared.Domain.Model.Entities;

namespace VitaliaBackend.Tenant.Domain.Model.Aggregates;

/// <summary>
///     Aggregate root for the organization that owns one or more branches.
/// </summary>
/// <remarks>
///     Uses a string <see cref="PublicId" /> (e.g. "hc-001") instead of exposing the
///     internal numeric <see cref="Id" />, the same pattern Scheduling already uses for
///     Appointment/AvailabilitySlot, because Branch refers to its healthcare center by
///     that same kind of business id, not by the database row number.
/// </remarks>
public class HealthcareCenter : IAuditableEntity
{
    public Guid Id { get; private set; }
    public string Code { get; private set; }
    public string Name { get; private set; }
    public DateOnly? AllianceStartDate { get; private set; }
    public DateOnly? AllianceFinishDate { get; private set; }
    public string? RucNumber { get; private set; }
    public string? ImageUrl { get; private set; }

    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    protected HealthcareCenter()
    {
        Code = string.Empty;
        Name = string.Empty;
    }

    public HealthcareCenter(
        string publicId,
        string name,
        DateOnly? allianceStartDate,
        DateOnly? allianceFinishDate,
        string? rucNumber,
        string? imageUrl = null)
    {
        Id = Guid.NewGuid();
        Code = publicId.Trim();
        Name = name.Trim();
        AllianceStartDate = allianceStartDate;
        AllianceFinishDate = allianceFinishDate;
        RucNumber = rucNumber?.Trim();
        ImageUrl = imageUrl?.Trim();
    }

    public HealthcareCenter(Guid id, string code, string name, DateOnly? allianceStartDate, DateOnly? allianceFinishDate, string? rucNumber, string? imageUrl = null)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        Code = code.Trim();
        Name = name.Trim();
        AllianceStartDate = allianceStartDate;
        AllianceFinishDate = allianceFinishDate;
        RucNumber = rucNumber?.Trim();
        ImageUrl = imageUrl?.Trim();
    }

    public void UpdateDetails(
        string name,
        DateOnly? allianceStartDate,
        DateOnly? allianceFinishDate,
        string? rucNumber,
        string? imageUrl = null)
    {
        Name = name.Trim();
        AllianceStartDate = allianceStartDate;
        AllianceFinishDate = allianceFinishDate;
        RucNumber = rucNumber?.Trim();
        ImageUrl = imageUrl?.Trim();
    }
}
