using VitaliaBackend.Shared.Domain.Model.ValueObjects;

namespace VitaliaBackend.Clinical.Interfaces.Rest.Resources;

public record DiagnosisCatalogEntryResource(
    DiagnosisCatalogSource Source,
    string Code,
    string Description
);
