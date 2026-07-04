using VitaliaBackend.Clinical.Application.CommandServices;
using VitaliaBackend.Clinical.Domain.Model.Aggregates;
using VitaliaBackend.Clinical.Domain.Repositories;
using VitaliaBackend.Clinical.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using VitaliaBackend.Shared.Domain.Model.ValueObjects;
using VitaliaBackend.Shared.Domain.Repositories;
using System.Text;

namespace VitaliaBackend.Clinical.Application.Internal.CommandServices;

public class DiagnosisCatalogImportService(
    IDiagnosisCatalogEntryRepository diagnosisCatalogEntryRepository,
    IUnitOfWork unitOfWork) : IDiagnosisCatalogImportService
{
    public async Task<int> ImportCsvAsync(
        Stream csvStream,
        DiagnosisCatalogSource source,
        CancellationToken cancellationToken)
    {
        using var reader = await DiagnosisCatalogCsvEncoding.CreateReaderAsync(csvStream, cancellationToken);
        var imported = 0;
        var lineNumber = 0;

        while (await reader.ReadLineAsync(cancellationToken) is { } line)
        {
            cancellationToken.ThrowIfCancellationRequested();

            lineNumber++;

            if (string.IsNullOrWhiteSpace(line))
                continue;

            var columns = ParseCsvLine(line);
            if (columns.Count < 2)
                continue;

            if (lineNumber == 1 &&
                columns[0].Trim().Equals("code", StringComparison.OrdinalIgnoreCase) &&
                columns[1].Trim().Equals("description", StringComparison.OrdinalIgnoreCase))
                continue;

            var code = columns[0].Trim().ToUpperInvariant();
            var description = columns[1].Trim();

            if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(description))
                continue;

            var searchText = DiagnosisCatalogSearchNormalizer.NormalizeForSearch($"{code} {description}");
            var existing = await diagnosisCatalogEntryRepository.FindBySourceAndCodeAsync(
                source,
                code,
                cancellationToken);

            if (existing is null)
            {
                await diagnosisCatalogEntryRepository.AddAsync(
                    new DiagnosisCatalogEntry(Guid.NewGuid(), source, code, description, searchText),
                    cancellationToken);
            }
            else
            {
                existing.UpdateDetails(description, searchText);
                diagnosisCatalogEntryRepository.Update(existing);
            }

            imported++;
        }

        await unitOfWork.CompleteAsync(cancellationToken);
        return imported;
    }

    private static List<string> ParseCsvLine(string line)
    {
        var columns = new List<string>();
        var current = new StringBuilder();
        var inQuotes = false;

        for (var index = 0; index < line.Length; index++)
        {
            var character = line[index];

            if (character == '"')
            {
                if (inQuotes && index + 1 < line.Length && line[index + 1] == '"')
                {
                    current.Append('"');
                    index++;
                    continue;
                }

                inQuotes = !inQuotes;
                continue;
            }

            if (character == ',' && !inQuotes)
            {
                columns.Add(current.ToString());
                current.Clear();
                continue;
            }

            current.Append(character);
        }

        columns.Add(current.ToString());
        return columns;
    }
}
