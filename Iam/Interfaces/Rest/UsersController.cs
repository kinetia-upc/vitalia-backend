using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitaliaBackend.Iam.Domain.Model.Aggregates;
using VitaliaBackend.Iam.Interfaces.Rest.Resources;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

namespace VitaliaBackend.Iam.Interfaces.Rest;

[ApiController]
[Authorize]
[Route("api/v1/users")]
[Produces(MediaTypeNames.Application.Json)]
public class UsersController(AppDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetUsers(CancellationToken cancellationToken)
    {
        var users = await context.Users.AsNoTracking().ToListAsync(cancellationToken);
        return Ok(users.Select(ToResource));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUserById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var user = await context.Users.AsNoTracking().FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        return user is null ? NotFound() : Ok(ToResource(user));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateUser([FromRoute] Guid id, [FromBody] UserResource resource, CancellationToken cancellationToken)
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
