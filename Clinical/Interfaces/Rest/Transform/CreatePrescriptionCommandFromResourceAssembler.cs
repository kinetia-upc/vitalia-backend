using VitaliaBackend.Clinical.Domain.Model.Commands;
using VitaliaBackend.Clinical.Interfaces.Rest.Resources;

namespace VitaliaBackend.Clinical.Interfaces.Rest.Transform;

public static class CreatePrescriptionCommandFromResourceAssembler
{
    public static CreatePrescriptionCommand ToCommandFromResource(CreatePrescriptionResource resource)
    {
        return new CreatePrescriptionCommand(resource.MedicalRecordId);
    }
}
