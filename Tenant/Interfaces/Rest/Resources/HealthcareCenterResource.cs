using Swashbuckle.AspNetCore.Annotations;

namespace VitaliaBackend.Tenant.Interfaces.Rest.Resources;

/// <summary>
///     Shape of the JSON the backend returns to the frontend for a healthcare center.
///     "id" here is the business PublicId (e.g. "hc-001"), not the internal numeric id.
/// </summary>
public record HealthcareCenterResource(
    [property: SwaggerSchema(Description = "Business id of the healthcare center, e.g. 'hc-001'.")]
    string Id,

    [property: SwaggerSchema(Description = "Name of the healthcare center organization.")]
    string HealthcareCenterName,

    [property: SwaggerSchema(Description = "Date the alliance/partnership with this healthcare center started.")]
    DateOnly? AllianceStartDate,

    [property: SwaggerSchema(Description = "Date the alliance/partnership with this healthcare center ends.")]
    DateOnly? AllianceFinishDate,

    [property: SwaggerSchema(Description = "Peruvian tax id (RUC) of the healthcare center, if registered as a business.")]
    long? RucNumber
);
