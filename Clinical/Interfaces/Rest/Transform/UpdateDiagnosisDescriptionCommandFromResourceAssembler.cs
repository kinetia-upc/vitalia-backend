using VitaliaBackend.Clinical.Domain.Model.Commands;
using VitaliaBackend.Clinical.Interfaces.Rest.Resources;

namespace VitaliaBackend.Clinical.Interfaces.Rest.Transform;

public static class UpdateDiagnosisDescriptionCommandFromResourceAssembler
{
    public static UpdateDiagnosisDescriptionCommand ToCommandFromResource(
        int diagnosisId,
        UpdateDescriptionResource resource)
    {
        return new UpdateDiagnosisDescriptionCommand(diagnosisId, resource.Description);
    }
}
