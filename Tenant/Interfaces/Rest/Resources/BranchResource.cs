using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;

namespace VitaliaBackend.Tenant.Interfaces.Rest.Resources;

public record BranchResource(
    [property: JsonPropertyName("id")]
    [property: SwaggerSchema(Description = "Business id of the branch, e.g. 'branch-001'.")]
    string Id,

    [property: JsonPropertyName("id_healthcare_center")]
    [property: SwaggerSchema(Description = "Business id of the healthcare center this branch belongs to.")]
    string IdHealthcareCenter,

    [property: JsonPropertyName("id_address")]
    [property: SwaggerSchema(Description = "Business id of the address record for this branch, if registered.")]
    string? IdAddress,

    [property: JsonPropertyName("branch_name")]
    [property: SwaggerSchema(Description = "Display name of the branch, e.g. 'Sede San Borja'.")]
    string BranchName,

    [property: JsonPropertyName("address")]
    [property: SwaggerSchema(Description = "Full street address of the branch.")]
    string Address
);
