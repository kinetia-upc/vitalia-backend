using VitaliaBackend.Shared.Domain.Model.ValueObjects;

namespace VitaliaBackend.Clinical.Application.Model;

public record DiagnosisCatalogItem(
    DiagnosisCatalogSource Source,
    string Code,
    string Description,
    string? Version = null,
    string? ExternalUri = null
);
