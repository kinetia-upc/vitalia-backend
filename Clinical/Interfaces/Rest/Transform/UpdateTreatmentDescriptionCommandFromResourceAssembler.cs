using VitaliaBackend.Clinical.Domain.Model.Commands;
using VitaliaBackend.Clinical.Interfaces.Rest.Resources;

namespace VitaliaBackend.Clinical.Interfaces.Rest.Transform;

public static class UpdateTreatmentDescriptionCommandFromResourceAssembler
{
    public static UpdateTreatmentDescriptionCommand ToCommandFromResource(
        Guid treatmentId,
        UpdateDescriptionResource resource)
    {
        return new UpdateTreatmentDescriptionCommand(treatmentId, resource.Description);
    }
}
