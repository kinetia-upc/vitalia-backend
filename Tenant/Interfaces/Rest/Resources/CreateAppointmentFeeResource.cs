using Swashbuckle.AspNetCore.Annotations;

namespace VitaliaBackend.Tenant.Interfaces.Rest.Resources;

public record CreateAppointmentFeeResource(
    [property: SwaggerSchema(Description = "Visible business code of the appointment fee, e.g. 'fee-001'. Client-supplied, must be unique.")]
    string Code,

    [property: SwaggerSchema(Description = "Business id of the branch this fee applies to.")]
    string BranchId,

    [property: SwaggerSchema(Description = "Business id of the medical speciality this fee applies to, if set.")]
    string? SpecialityId,

    [property: SwaggerSchema(Description = "Price charged for an appointment. Must be zero or greater.")]
    decimal Price
);
