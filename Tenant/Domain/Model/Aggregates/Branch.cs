using VitaliaBackend.Shared.Domain.Model.Entities;

namespace VitaliaBackend.Tenant.Domain.Model.Aggregates;

/// <summary>
///     A physical location of a <see cref="HealthcareCenter" />.
/// </summary>
/// <remarks>
///     <see cref="AddressId" /> points to an Address record that lives outside this
///     bounded context's scope, so it is kept as a plain string reference instead of
///     a navigation property, the same way <see cref="HealthcareCenterId" /> refers to
///     its parent only by id.
/// </remarks>
public class Branch : IAuditableEntity
{
    public int Id { get; private set; }
    public string PublicId { get; private set; }
    public string HealthcareCenterId { get; private set; }
    public string? AddressId { get; private set; }
    public string Name { get; private set; }
    public string Address { get; private set; }

    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    protected Branch()
    {
        PublicId = string.Empty;
        HealthcareCenterId = string.Empty;
        Name = string.Empty;
        Address = string.Empty;
    }

    public Branch(
        string publicId,
        string healthcareCenterId,
        string? addressId,
        string name,
        string address)
    {
        PublicId = publicId.Trim();
        HealthcareCenterId = healthcareCenterId.Trim();
        AddressId = addressId?.Trim();
        Name = name.Trim();
        Address = address.Trim();
    }

    public void UpdateDetails(
        string healthcareCenterId,
        string? addressId,
        string name,
        string address)
    {
        HealthcareCenterId = healthcareCenterId.Trim();
        AddressId = addressId?.Trim();
        Name = name.Trim();
        Address = address.Trim();
    }
}
