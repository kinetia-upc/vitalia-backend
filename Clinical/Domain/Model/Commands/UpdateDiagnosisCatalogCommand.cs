namespace VitaliaBackend.Clinical.Domain.Model.Commands;

public record UpdateDiagnosisCatalogCommand(Guid DiagnosisId, string Cie10Code, string Description);
