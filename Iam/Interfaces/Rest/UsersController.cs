using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using VitaliaBackend.Iam.Domain.Model.Aggregates;
using VitaliaBackend.Iam.Infrastructure.Security;
using VitaliaBackend.Iam.Interfaces.Rest.Resources;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

namespace VitaliaBackend.Iam.Interfaces.Rest;

[ApiController]
[Route("api/v1/users")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Users endpoints")]
public class UsersController(AppDbContext context) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "List users", Description = "Returns every IAM user registered in the platform.")]
    [SwaggerResponse(200, "The list of users was returned.", typeof(IEnumerable<UserResource>))]
    public async Task<IActionResult> GetUsers(CancellationToken cancellationToken)
    {
        var users = await context.Users.AsNoTracking().ToListAsync(cancellationToken);
        return Ok(users.Select(ToResource));
    }

    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Get a user by id", Description = "Returns a single IAM user identified by its UUID.")]
    [SwaggerResponse(200, "The user was found.", typeof(UserResource))]
    [SwaggerResponse(404, "No user exists with the given id.")]
    public async Task<IActionResult> GetUserById(
        [FromRoute, SwaggerParameter("UUID of the user.")]
        Guid id,
        CancellationToken cancellationToken)
    {
        var user = await context.Users.AsNoTracking().FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        return user is null ? NotFound() : Ok(ToResource(user));
    }

    [HttpPut("{id:guid}")]
    [SwaggerOperation(Summary = "Update a user", Description = "Replaces profile, contact and role fields for an existing IAM user.")]
    [SwaggerResponse(200, "The user was updated.", typeof(UserResource))]
    [SwaggerResponse(404, "No user exists with the given id.")]
    public async Task<IActionResult> UpdateUser(
        [FromRoute, SwaggerParameter("UUID of the user to update.")]
        Guid id,
        [FromBody, SwaggerParameter("New data for the user.")]
        UserResource resource,
        CancellationToken cancellationToken)
    {
        var user = await context.Users.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (user is null) return NotFound();

        user.UpdateDetails(
            resource.HealthcareCenterId,
            resource.Name,
            resource.PaternalSurname,
            resource.MaternalSurname,
            resource.IdentityType,
            resource.IdentityNumber,
            resource.DateBirth,
            resource.Email,
            resource.Phone,
            resource.Gender,
            resource.IsActive,
            resource.Address,
            resource.Role);

        if (!string.IsNullOrWhiteSpace(resource.Password))
            user.UpdatePasswordHash(PasswordHashingService.Hash(resource.Password));

        await context.SaveChangesAsync(cancellationToken);
        return Ok(ToResource(user));
    }

    private static UserResource ToResource(User user) => new(
        user.Id,
        user.HealthcareCenterId,
        user.Name,
        user.PaternalSurname,
        user.MaternalSurname,
        user.IdentityType,
        user.IdentityNumber,
        user.BirthDate,
        user.Email,
        user.Phone,
        user.Gender,
        user.IsActive,
        user.Address,
        user.Role);
}
