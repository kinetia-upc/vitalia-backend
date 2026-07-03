using VitaliaBackend.Clinical.Domain.Model.Commands;
using VitaliaBackend.Clinical.Interfaces.Rest.Resources;

namespace VitaliaBackend.Clinical.Interfaces.Rest.Transform;

public static class UpdatePrescriptionDetailCommandFromResourceAssembler
{
    public static UpdatePrescriptionDetailCommand ToCommandFromResource(
        Guid prescriptionId,
        Guid medicineId,
        UpdatePrescriptionDetailResource resource)
    {
        return new UpdatePrescriptionDetailCommand(
            prescriptionId,
            medicineId,
            resource.Quantity,
            resource.Frequency,
            resource.Duration);
    }
}
