using VitaliaBackend.Clinical.Domain.Model.Commands;
using VitaliaBackend.Clinical.Interfaces.Rest.Resources;

namespace VitaliaBackend.Clinical.Interfaces.Rest.Transform;

public static class CreateTreatmentCommandFromResourceAssembler
{
    public static CreateTreatmentCommand ToCommandFromResource(CreateTreatmentResource resource)
    {
        return new CreateTreatmentCommand(resource.Code, resource.MedicalRecordId, resource.Description);
    }
}
