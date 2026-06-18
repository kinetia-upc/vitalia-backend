namespace VitaliaBackend.Clinical.Domain.Model.Commands;

public record UpdateTreatmentDescriptionCommand(int TreatmentId, string Description);
