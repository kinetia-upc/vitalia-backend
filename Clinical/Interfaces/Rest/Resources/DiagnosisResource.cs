namespace VitaliaBackend.Clinical.Interfaces.Rest.Resources;

using VitaliaBackend.Shared.Domain.Model.ValueObjects;

public record DiagnosisResource(
    Guid Id,
    string Code,
    Guid MedicalRecordId,
    string Cie10Code,
    string Description,
    DiagnosisCatalogSource DiagnosisCatalogSource,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt
);
