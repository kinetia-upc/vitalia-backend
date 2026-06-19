using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;

namespace VitaliaBackend.Tenant.Interfaces.Rest.Resources;

public record UpdateAppointmentFeeResource(
    [property: JsonPropertyName("id_branch")]
    [property: SwaggerSchema(Description = "Business id of the branch this fee applies to.")]
    string IdBranch,

    [property: JsonPropertyName("id_speciality")]
    [property: SwaggerSchema(Description = "Business id of the medical speciality this fee applies to, if set.")]
    string? IdSpeciality,

    [property: JsonPropertyName("price")]
    [property: SwaggerSchema(Description = "Price charged for an appointment. Must be zero or greater.")]
    decimal Price
);
