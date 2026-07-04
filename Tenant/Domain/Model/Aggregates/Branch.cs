using VitaliaBackend.Shared.Domain.Model.Entities;
using VitaliaBackend.Shared.Domain.Model.ValueObjects;

namespace VitaliaBackend.Tenant.Domain.Model.Aggregates;

/// <summary>
///     A physical location of a <see cref="HealthcareCenter" />.
/// </summary>
public class Branch : IAuditableEntity
{
    public Guid Id { get; private set; }
    public string Code { get; private set; }
    public string HealthcareCenterId { get; private set; }
    public string Name { get; private set; }
    public string Address { get; private set; }
    public DiagnosisCatalogSource DiagnosisCatalogSource { get; private set; }

    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    protected Branch()
    {
        Code = string.Empty;
        HealthcareCenterId = string.Empty;
        Name = string.Empty;
        Address = string.Empty;
        DiagnosisCatalogSource = DiagnosisCatalogSource.MINSA_CIE10;
    }

    public Branch(
        string publicId,
        string healthcareCenterId,
        string name,
        string address,
        DiagnosisCatalogSource diagnosisCatalogSource = DiagnosisCatalogSource.MINSA_CIE10)
    {
        Id = Guid.NewGuid();
        Code = publicId.Trim();
        HealthcareCenterId = healthcareCenterId.Trim();
        Name = name.Trim();
        Address = address.Trim();
        DiagnosisCatalogSource = diagnosisCatalogSource;
    }

    public Branch(
        Guid id,
        string code,
        string healthcareCenterId,
        string name,
        string address,
        DiagnosisCatalogSource diagnosisCatalogSource = DiagnosisCatalogSource.MINSA_CIE10)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        Code = code.Trim();
        HealthcareCenterId = healthcareCenterId.Trim();
        Name = name.Trim();
        Address = address.Trim();
        DiagnosisCatalogSource = diagnosisCatalogSource;
    }

    public void UpdateDetails(
        string healthcareCenterId,
        string name,
        string address,
        DiagnosisCatalogSource diagnosisCatalogSource = DiagnosisCatalogSource.MINSA_CIE10)
    {
        HealthcareCenterId = healthcareCenterId.Trim();
        Name = name.Trim();
        Address = address.Trim();
        DiagnosisCatalogSource = diagnosisCatalogSource;
    }
}
