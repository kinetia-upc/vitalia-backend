namespace VitaliaBackend.Clinical.Domain.Model.Commands;

public record DeletePrescriptionDetailCommand(Guid PrescriptionId, Guid MedicineId);
