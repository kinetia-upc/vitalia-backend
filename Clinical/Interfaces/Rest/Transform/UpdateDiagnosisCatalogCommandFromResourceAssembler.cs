using VitaliaBackend.Clinical.Domain.Model.Commands;
using VitaliaBackend.Clinical.Interfaces.Rest.Resources;

namespace VitaliaBackend.Clinical.Interfaces.Rest.Transform;

public static class UpdateDiagnosisCatalogCommandFromResourceAssembler
{
    public static UpdateDiagnosisCatalogCommand ToCommandFromResource(
        Guid diagnosisId,
        UpdateDiagnosisCatalogResource resource)
    {
        return new UpdateDiagnosisCatalogCommand(diagnosisId, resource.Cie10Code, resource.Description);
    }
}
