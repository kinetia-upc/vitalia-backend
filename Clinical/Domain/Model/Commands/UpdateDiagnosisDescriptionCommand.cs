namespace VitaliaBackend.Clinical.Domain.Model.Commands;

public record UpdateDiagnosisDescriptionCommand(int DiagnosisId, string Description);
