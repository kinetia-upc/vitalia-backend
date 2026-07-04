using VitaliaBackend.Clinical.Application.Model;
using VitaliaBackend.Clinical.Interfaces.Rest.Resources;

namespace VitaliaBackend.Clinical.Interfaces.Rest.Transform;

public static class DiagnosisCatalogEntryResourceFromItemAssembler
{
    public static DiagnosisCatalogEntryResource ToResourceFromItem(DiagnosisCatalogItem item)
    {
        return new DiagnosisCatalogEntryResource(item.Source, item.Code, item.Description);
    }
}
