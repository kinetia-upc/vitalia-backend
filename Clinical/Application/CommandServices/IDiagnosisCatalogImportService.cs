using VitaliaBackend.Shared.Domain.Model.ValueObjects;

namespace VitaliaBackend.Clinical.Application.CommandServices;

public interface IDiagnosisCatalogImportService
{
    Task<int> ImportCsvAsync(
        Stream csvStream,
        DiagnosisCatalogSource source,
        CancellationToken cancellationToken);
}
