using VitaliaBackend.Clinical.Domain.Model.Commands;
using VitaliaBackend.Clinical.Interfaces.Rest.Resources;

namespace VitaliaBackend.Clinical.Interfaces.Rest.Transform;

public static class UpdatePrescriptionDetailCommandFromResourceAssembler
{
    public static UpdatePrescriptionDetailCommand ToCommandFromResource(
        int prescriptionDetailId,
        UpdatePrescriptionDetailResource resource)
    {
        return new UpdatePrescriptionDetailCommand(
            prescriptionDetailId,
            resource.MedicineId,
            resource.MedicineName,
            resource.DoseAmount,
            resource.DoseUnit,
            resource.Frequency,
            resource.Duration);
    }
}
