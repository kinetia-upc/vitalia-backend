using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;

namespace VitaliaBackend.Tenant.Interfaces.Rest.Resources;

/// <summary>
///     Shape of the JSON the backend returns to the frontend for a healthcare center.
///     "id" here is the business PublicId (e.g. "hc-001"), not the internal numeric id.
/// </summary>
public record HealthcareCenterResource(
    [property: JsonPropertyName("id")]
    [property: SwaggerSchema(Description = "Business id of the healthcare center, e.g. 'hc-001'.")]
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
