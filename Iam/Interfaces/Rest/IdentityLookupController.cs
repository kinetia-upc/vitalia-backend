using System.Net;
using System.Net.Mime;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace VitaliaBackend.Iam.Interfaces.Rest;

[ApiController]
[Route("api/v1/identity")]
[Produces(MediaTypeNames.Application.Json)]
[AllowAnonymous]
[SwaggerTag("Identity lookup endpoints")]
public class IdentityLookupController(
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration) : ControllerBase
{
    private const string DefaultDniUrlTemplate = "https://peruapi.com/api/dni/{dni}?api_token={apiKey}";

    [HttpGet("dni/{dni}")]
    [SwaggerOperation(Summary = "Lookup a Peruvian DNI", Description = "Returns first names and surnames for a valid DNI.")]
    [SwaggerResponse(200, "DNI data found.", typeof(DniLookupResource))]
    [SwaggerResponse(400, "DNI is invalid.")]
    [SwaggerResponse(404, "DNI was not found.")]
    public async Task<IActionResult> GetDni([FromRoute] string dni, CancellationToken cancellationToken)
    {
        var normalizedDni = new string((dni ?? string.Empty).Where(char.IsDigit).ToArray());
        if (normalizedDni.Length != 8)
            return BadRequest(new { message = "DNI must have 8 digits." });

        var apiKey = configuration["PeruApi:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
            return StatusCode((int)HttpStatusCode.ServiceUnavailable, new { message = "Peru API key is not configured." });

        var urlTemplate = configuration["PeruApi:DniUrlTemplate"] ?? DefaultDniUrlTemplate;
        var url = urlTemplate
            .Replace("{dni}", Uri.EscapeDataString(normalizedDni))
            .Replace("{apiKey}", Uri.EscapeDataString(apiKey));

        using var httpClient = httpClientFactory.CreateClient();
        httpClient.Timeout = TimeSpan.FromSeconds(8);

        using var response = await httpClient.GetAsync(url, cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return NotFound(new { message = "DNI was not found." });

        if (!response.IsSuccessStatusCode)
            return StatusCode((int)HttpStatusCode.BadGateway, new { message = "DNI provider is unavailable." });

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

        if (TryGetProperty(document.RootElement, "code", out var code)
            && code.ValueKind == JsonValueKind.String
            && code.GetString() is { } providerCode
            && providerCode != "200")
            return StatusCode((int)HttpStatusCode.BadGateway, new { message = ReadString(document.RootElement, "mensaje", "message") });

        var resource = ToDniLookupResource(normalizedDni, document.RootElement);

        if (string.IsNullOrWhiteSpace(resource.FirstNames)
            && string.IsNullOrWhiteSpace(resource.PaternalSurname)
            && string.IsNullOrWhiteSpace(resource.MaternalSurname))
            return NotFound(new { message = "DNI was not found." });

        return Ok(resource);
    }

    private static DniLookupResource ToDniLookupResource(string dni, JsonElement root)
    {
        var payload = TryGetProperty(root, "data", out var data) && data.ValueKind == JsonValueKind.Object
            ? data
            : root;

        var firstNames = ReadString(payload, "nombres", "names", "firstNames", "prenombres");
        var paternalSurname = ReadString(payload, "apellido_paterno", "apellidoPaterno", "paternalSurname");
        var maternalSurname = ReadString(payload, "apellido_materno", "apellidoMaterno", "maternalSurname");
        var fullName = ReadString(payload, "nombre_completo", "nombreCompleto", "fullName");

        if (string.IsNullOrWhiteSpace(firstNames) && !string.IsNullOrWhiteSpace(fullName))
        {
            var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 3)
            {
                paternalSurname = string.IsNullOrWhiteSpace(paternalSurname) ? parts[0] : paternalSurname;
                maternalSurname = string.IsNullOrWhiteSpace(maternalSurname) ? parts[1] : maternalSurname;
                firstNames = string.Join(' ', parts.Skip(2));
            }
        }

        return new DniLookupResource(dni, firstNames, paternalSurname, maternalSurname, fullName);
    }

    private static string ReadString(JsonElement element, params string[] propertyNames)
    {
        foreach (var propertyName in propertyNames)
        {
            if (TryGetProperty(element, propertyName, out var property) && property.ValueKind == JsonValueKind.String)
                return property.GetString()?.Trim() ?? string.Empty;
        }

        return string.Empty;
    }

    private static bool TryGetProperty(JsonElement element, string propertyName, out JsonElement property)
    {
        foreach (var item in element.EnumerateObject())
        {
            if (item.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
            {
                property = item.Value;
                return true;
            }
        }

        property = default;
        return false;
    }
}

public record DniLookupResource(
    string Dni,
    string FirstNames,
    string PaternalSurname,
    string MaternalSurname,
    string FullName);
