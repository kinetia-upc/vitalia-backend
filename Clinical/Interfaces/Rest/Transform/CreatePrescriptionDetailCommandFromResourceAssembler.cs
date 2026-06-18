using VitaliaBackend.Clinical.Domain.Model.Commands;
using VitaliaBackend.Clinical.Interfaces.Rest.Resources;

namespace VitaliaBackend.Clinical.Interfaces.Rest.Transform;

public static class CreatePrescriptionDetailCommandFromResourceAssembler
{
    public static CreatePrescriptionDetailCommand ToCommandFromResource(CreatePrescriptionDetailResource resource)
    {
        return new CreatePrescriptionDetailCommand(
            resource.PrescriptionId,
            resource.MedicineId,
            resource.MedicineName,
            resource.DoseAmount,
            resource.DoseUnit,
            resource.Frequency,
            resource.Duration);
    }
}
