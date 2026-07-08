using Swashbuckle.AspNetCore.Annotations;

namespace VitaliaBackend.Tenant.Interfaces.Rest.Resources;

/// <summary>
///     Shape of the JSON the backend returns to the frontend for a healthcare center.
///     Exposes both the internal UUID and the visible business code.
/// </summary>
public record HealthcareCenterResource(
    [property: SwaggerSchema(Description = "Internal UUID of the healthcare center.")]
    Guid Id,

    [property: SwaggerSchema(Description = "Visible business code of the healthcare center, e.g. 'hc-001'.")]
    string Code,

    [property: SwaggerSchema(Description = "Name of the healthcare center organization.")]
    string HealthcareCenterName,

    [property: SwaggerSchema(Description = "Date the alliance/partnership with this healthcare center started.")]
    DateOnly? AllianceStartDate,

    [property: SwaggerSchema(Description = "Date the alliance/partnership with this healthcare center ends.")]
    DateOnly? AllianceFinishDate,

    [property: SwaggerSchema(Description = "Peruvian tax id (RUC) of the healthcare center, if registered as a business.")]
    string? RucNumber,

    [property: SwaggerSchema(Description = "Optional public image URL used as the healthcare center brand logo.")]
    string? ImageUrl
);
