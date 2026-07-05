using VitaliaBackend.Clinical.Application.Model;
using VitaliaBackend.Clinical.Domain.Services;
using VitaliaBackend.Shared.Domain.Model.ValueObjects;
using System.Text.Json;

namespace VitaliaBackend.Clinical.Infrastructure.DiagnosisCatalog;

public class WhoDiagnosisCatalogProvider(HttpClient httpClient) : IDiagnosisCatalogProvider
{
    private const int DefaultLimit = 7;
    private const int MaxLimit = 50;

    public async Task<IReadOnlyCollection<DiagnosisCatalogItem>> SearchAsync(
        DiagnosisCatalogSource source,
        string? query,
        int limit,
        CancellationToken cancellationToken)
    {
        if (!IsExternal(source) || string.IsNullOrWhiteSpace(query))
            return [];

        return await SearchClinicalTablesAsync(query.Trim(), NormalizeLimit(limit), cancellationToken);
    }

    public async Task<DiagnosisCatalogItem?> FindByCodeAsync(
        DiagnosisCatalogSource source,
        string code,
        CancellationToken cancellationToken)
    {
        if (!IsExternal(source) || string.IsNullOrWhiteSpace(code))
            return null;

        var normalizedCode = code.Trim();
        var items = await SearchClinicalTablesAsync(normalizedCode, DefaultLimit, cancellationToken, searchField: "code");

        return items.FirstOrDefault(item =>
            string.Equals(item.Code, normalizedCode, StringComparison.OrdinalIgnoreCase));
    }

    private async Task<IReadOnlyCollection<DiagnosisCatalogItem>> SearchClinicalTablesAsync(
        string terms,
        int limit,
        CancellationToken cancellationToken,
        string searchField = "code,name")
    {
        var requestUri =
            $"search?sf={Uri.EscapeDataString(searchField)}&df=code,name&terms={Uri.EscapeDataString(terms)}&count={limit}";

        using var response = await httpClient.GetAsync(requestUri, cancellationToken);
        if (!response.IsSuccessStatusCode)
            return [];

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

        return ParseClinicalTablesResponse(document.RootElement);
    }

    private static IReadOnlyCollection<DiagnosisCatalogItem> ParseClinicalTablesResponse(JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Array || root.GetArrayLength() < 4)
            return [];

        var codes = root[1];
        var displayRows = root[3];
        if (codes.ValueKind != JsonValueKind.Array || displayRows.ValueKind != JsonValueKind.Array)
            return [];

        var count = Math.Min(codes.GetArrayLength(), displayRows.GetArrayLength());
        var items = new List<DiagnosisCatalogItem>(count);

        for (var index = 0; index < count; index++)
        {
            var code = codes[index].GetString();
            var description = ReadDescription(displayRows[index], code);

            if (!string.IsNullOrWhiteSpace(code) && !string.IsNullOrWhiteSpace(description))
                items.Add(new DiagnosisCatalogItem(
                    DiagnosisCatalogSource.WHO_CIE10,
                    code,
                    description,
                    ExternalUri: $"https://clinicaltables.nlm.nih.gov/api/icd10cm/v3/search?terms={Uri.EscapeDataString(code)}"));
        }

        return items;
    }

    private static string? ReadDescription(JsonElement displayRow, string? fallbackCode)
    {
        if (displayRow.ValueKind != JsonValueKind.Array)
            return null;

        if (displayRow.GetArrayLength() > 1)
            return displayRow[1].GetString();

        if (displayRow.GetArrayLength() == 1)
            return displayRow[0].GetString() == fallbackCode ? null : displayRow[0].GetString();

        return null;
    }

    private static int NormalizeLimit(int limit)
    {
        return limit <= 0 ? DefaultLimit : Math.Min(limit, MaxLimit);
    }

    private static bool IsExternal(DiagnosisCatalogSource source)
    {
        return source == DiagnosisCatalogSource.WHO_CIE10;
    }
}
