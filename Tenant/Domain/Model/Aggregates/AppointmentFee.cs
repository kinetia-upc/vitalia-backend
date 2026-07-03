using VitaliaBackend.Shared.Domain.Model.Entities;

namespace VitaliaBackend.Tenant.Domain.Model.Aggregates;

/// <summary>
///     The price the clinic charges for an appointment at a given branch and
///     speciality. <see cref="SpecialityId" /> refers to a Speciality record that
///     lives outside this bounded context's scope, kept as a plain string reference.
/// </summary>
public class AppointmentFee : IAuditableEntity
{
    public Guid Id { get; private set; }
    public string Code { get; private set; }
    public string BranchId { get; private set; }
    public string? SpecialityId { get; private set; }
    public decimal Price { get; private set; }

    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    protected AppointmentFee()
    {
        Code = string.Empty;
        BranchId = string.Empty;
    }

    public AppointmentFee(
        string publicId,
        string branchId,
        string? specialityId,
        decimal price)
    {
        Id = Guid.NewGuid();
        Code = publicId.Trim();
        BranchId = branchId.Trim();
        SpecialityId = specialityId?.Trim();
        Price = price;
    }

    public AppointmentFee(Guid id, string code, string branchId, string? specialityId, decimal price)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        Code = code.Trim();
        BranchId = branchId.Trim();
        SpecialityId = specialityId?.Trim();
        Price = price;
    }

    public void UpdateDetails(
        string branchId,
        string? specialityId,
        decimal price)
    {
        BranchId = branchId.Trim();
        SpecialityId = specialityId?.Trim();
        Price = price;
    }
}
