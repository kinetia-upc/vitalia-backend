using VitaliaBackend.Clinical.Domain.Model.Commands;
using VitaliaBackend.Clinical.Interfaces.Rest.Resources;

namespace VitaliaBackend.Clinical.Interfaces.Rest.Transform;

public static class CreateDiagnosisCommandFromResourceAssembler
{
    public static CreateDiagnosisCommand ToCommandFromResource(CreateDiagnosisResource resource)
    {
        return new CreateDiagnosisCommand(resource.MedicalRecordId, resource.Description);
    }
}
