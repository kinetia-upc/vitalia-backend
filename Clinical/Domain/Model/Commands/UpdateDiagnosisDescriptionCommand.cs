namespace VitaliaBackend.Clinical.Domain.Model.Commands;

public record UpdateDiagnosisDescriptionCommand(Guid DiagnosisId, string Description);
