namespace VitaliaBackend.Clinical.Domain.Model.Queries;

public record GetPrescriptionDetailByIdQuery(Guid PrescriptionId, Guid MedicineId);
