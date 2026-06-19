using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;

namespace VitaliaBackend.Tenant.Interfaces.Rest.Resources;

/// <summary>
///     Shape of the JSON body the frontend sends when creating a healthcare center.
/// </summary>
/// <remarks>
///     Field names use snake_case (e.g. "healthcare_center_name") because that is the
///     exact shape vitalia-frontend/src/modules/tenant expects, unlike Billing which
///     uses camelCase. [JsonPropertyName] overrides ASP.NET Core's default camelCase
///     serialization for each field.
/// </remarks>
public record CreateHealthcareCenterResource(
    [property: JsonPropertyName("id")]
    [property: SwaggerSchema(Description = "Business id of the healthcare center, e.g. 'hc-001'. Client-supplied, must be unique.")]
    string Id,

    [property: JsonPropertyName("healthcare_center_name")]
    [property: SwaggerSchema(Description = "Name of the healthcare center organization.")]
    string HealthcareCenterName,

    [property: JsonPropertyName("alliance_start_date")]
    [property: SwaggerSchema(Description = "Date the alliance/partnership with this healthcare center started.")]
    DateOnly? AllianceStartDate,

    [property: JsonPropertyName("alliance_finish_date")]
    [property: SwaggerSchema(Description = "Date the alliance/partnership with this healthcare center ends.")]
    DateOnly? AllianceFinishDate,

    [property: JsonPropertyName("ruc_number")]
    [property: SwaggerSchema(Description = "Peruvian tax id (RUC) of the healthcare center, if registered as a business.")]
    long? RucNumber
);
