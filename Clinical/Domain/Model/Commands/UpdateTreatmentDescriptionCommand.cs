namespace VitaliaBackend.Clinical.Domain.Model.Commands;

public record UpdateTreatmentDescriptionCommand(Guid TreatmentId, string Description);
