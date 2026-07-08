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
[Route("api/v1/authentication")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Authentication endpoints")]
public class AuthenticationController(AppDbContext context, IConfiguration configuration) : ControllerBase
{
    private static readonly HashSet<string> AllowedRoles = ["admin", "doctor", "patient"];

    [HttpPost("signIn")]
    [SwaggerOperation(Summary = "Sign in", Description = "Authenticates an active user and returns a JWT bearer token.")]
    [SwaggerResponse(200, "The user was authenticated.", typeof(AuthenticatedUserResource))]
    [SwaggerResponse(401, "The email or password is invalid, or the user is inactive.")]
    public async Task<IActionResult> SignIn(
        [FromBody, SwaggerParameter("Credentials used to authenticate the user.")]
        SignInResource resource,
        CancellationToken cancellationToken)
    {
        var email = resource.Email.Trim().ToLowerInvariant();
        var user = await context.Users.FirstOrDefaultAsync(item => item.Email == email, cancellationToken);
        if (user is null || !user.IsActive || !PasswordHashingService.Verify(resource.Password, user.PasswordHash))
            return Unauthorized();

        var token = JwtTokenService.Generate(user, configuration);
        return Ok(new AuthenticatedUserResource(ToResource(user), token));
    }

    [HttpPost("signUp")]
    [SwaggerOperation(Summary = "Sign up", Description = "Registers a new active user and stores a hashed password.")]
    [SwaggerResponse(201, "The user was registered.", typeof(UserResource))]
    [SwaggerResponse(400, "The requested role is not supported.")]
    [SwaggerResponse(409, "A user with the same email already exists.")]
    public async Task<IActionResult> SignUp(
        [FromBody, SwaggerParameter("Data for the new user account.")]
        SignUpResource resource,
        CancellationToken cancellationToken)
    {
        var email = resource.Email.Trim().ToLowerInvariant();
        var role = resource.Role.Trim().ToLowerInvariant();

        if (!AllowedRoles.Contains(role))
            return BadRequest(new { message = "Role must be one of: admin, doctor, patient." });

        if (await context.Users.AnyAsync(item => item.Email == email, cancellationToken))
            return Conflict(new { message = "A user with this email already exists." });

        var user = new User(
            Guid.NewGuid(),
            resource.HealthcareCenterId,
            resource.Name,
            resource.PaternalSurname,
            resource.MaternalSurname,
            resource.IdentityType,
            resource.IdentityNumber,
            resource.DateBirth,
            email,
            PasswordHashingService.Hash(resource.Password),
            resource.Phone,
            resource.Gender,
            true,
            resource.Address,
            role);

        context.Users.Add(user);
        await context.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(UsersController.GetUserById), "Users", new { id = user.Id }, ToResource(user));
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
