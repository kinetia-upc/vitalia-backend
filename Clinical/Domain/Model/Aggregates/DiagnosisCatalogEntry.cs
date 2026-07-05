using VitaliaBackend.Shared.Domain.Model.ValueObjects;

namespace VitaliaBackend.Clinical.Domain.Model.Aggregates;

public class DiagnosisCatalogEntry
{
    public Guid Id { get; private set; }
    public DiagnosisCatalogSource Source { get; private set; }
    public string Code { get; private set; }
    public string Description { get; private set; }
    public string SearchText { get; private set; }

    protected DiagnosisCatalogEntry()
    {
        Code = string.Empty;
        Description = string.Empty;
        SearchText = string.Empty;
    }

    public DiagnosisCatalogEntry(
        Guid id,
        DiagnosisCatalogSource source,
        string code,
        string description,
        string searchText)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        Source = source;
        Code = code.Trim();
        Description = description.Trim();
        SearchText = searchText.Trim();
    }

    public void UpdateDetails(string description, string searchText)
    {
        Description = description.Trim();
        SearchText = searchText.Trim();
    }
}
