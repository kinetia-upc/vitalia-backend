namespace VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Shared.Domain.Model.Entities;
using VitaliaBackend.Shared.Domain.Model.ValueObjects;

public class Diagnosis : IAuditableEntity
{
    public Guid Id { get; private set; }
    public string Code { get; private set; }
    public Guid MedicalRecordId { get; private set; }
    public string Cie10Code { get; private set; }
    public string Description { get; private set; }
    public DiagnosisCatalogSource DiagnosisCatalogSource { get; private set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    protected Diagnosis()
    {
        Code = string.Empty;
        Cie10Code = string.Empty;
        Description = string.Empty;
        DiagnosisCatalogSource = DiagnosisCatalogSource.MINSA_CIE10;
    }
    
    public Diagnosis(
        Guid id,
        string code,
        Guid medicalRecordId,
        string cie10Code,
        string description,
        DiagnosisCatalogSource diagnosisCatalogSource)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        Code = code.Trim();
        MedicalRecordId = medicalRecordId;
        Cie10Code = cie10Code.Trim();
        Description = description.Trim();
        DiagnosisCatalogSource = diagnosisCatalogSource;
    }

    public void UpdateCatalogDetails(
        string cie10Code,
        string description,
        DiagnosisCatalogSource diagnosisCatalogSource)
    {
        Cie10Code = cie10Code.Trim();
        Description = description.Trim();
        DiagnosisCatalogSource = diagnosisCatalogSource;
    }
}
