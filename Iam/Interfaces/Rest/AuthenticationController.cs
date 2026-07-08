using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitaliaBackend.Iam.Domain.Model.Aggregates;
using VitaliaBackend.Iam.Infrastructure.Security;
using VitaliaBackend.Iam.Interfaces.Rest.Resources;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using VitaliaBackend.Tenant.Domain.Model.Aggregates;

namespace VitaliaBackend.Iam.Interfaces.Rest;

[ApiController]
[Route("api/v1/authentication")]
[Produces(MediaTypeNames.Application.Json)]
public class AuthenticationController(AppDbContext context, IConfiguration configuration) : ControllerBase
{
    private static readonly HashSet<string> AllowedRoles = ["admin", "doctor", "patient"];

    [HttpPost("signIn")]
    public async Task<IActionResult> SignIn([FromBody] SignInResource resource, CancellationToken cancellationToken)
    {
        var email = resource.Email.Trim().ToLowerInvariant();
        var user = await context.Users.FirstOrDefaultAsync(item => item.Email == email, cancellationToken);
        if (user is null || !user.IsActive || !PasswordHashingService.Verify(resource.Password, user.PasswordHash))
            return Unauthorized();

        var token = JwtTokenService.Generate(user, configuration);
        return Ok(new AuthenticatedUserResource(ToResource(user), token));
    }

    [HttpPost("signUp")]
    public async Task<IActionResult> SignUp([FromBody] SignUpResource resource, CancellationToken cancellationToken)
    {
        var email = resource.Email.Trim().ToLowerInvariant();
        var role = resource.Role.Trim().ToLowerInvariant();
        var healthcareCenterId = resource.HealthcareCenterId?.Trim() ?? string.Empty;

        if (!AllowedRoles.Contains(role))
            return BadRequest(new { message = "Role must be one of: admin, doctor, patient." });

        if (string.IsNullOrWhiteSpace(healthcareCenterId))
            return BadRequest(new { message = "Healthcare center is required." });

        var healthcareCenterGuid = Guid.TryParse(healthcareCenterId, out var parsedHealthcareCenterId)
            ? parsedHealthcareCenterId
            : (Guid?)null;

        var healthcareCenter = await context.HealthcareCenters.FirstOrDefaultAsync(
            item => item.Code == healthcareCenterId || (healthcareCenterGuid.HasValue && item.Id == healthcareCenterGuid.Value),
            cancellationToken);

        if (healthcareCenter is null)
            return NotFound(new { message = "Healthcare center does not exist." });

        if (await context.Users.AnyAsync(item => item.Email == email, cancellationToken))
            return Conflict(new { message = "A user with this email already exists." });

        var user = new User(
            Guid.NewGuid(),
            healthcareCenter.Id.ToString(),
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

        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        context.Users.Add(user);
        if (role == "patient")
            await AddPatientProfileAsync(context, user.Id, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return CreatedAtAction(nameof(UsersController.GetUserById), "Users", new { id = user.Id }, ToResource(user));
    }

    private static async Task AddPatientProfileAsync(AppDbContext context, Guid userId, CancellationToken cancellationToken)
    {
        context.Patients.Add(new Patient(
            userId,
            await BuildNextPatientCodeAsync(context, cancellationToken),
            string.Empty,
            string.Empty,
            null,
            string.Empty,
            string.Empty));
    }

    private static async Task<string> BuildNextPatientCodeAsync(AppDbContext context, CancellationToken cancellationToken)
    {
        var codes = await context.Patients.AsNoTracking()
            .Select(patient => patient.Code)
            .ToListAsync(cancellationToken);

        var nextNumber = codes
            .Select(ParsePatientCodeNumber)
            .DefaultIfEmpty(0)
            .Max() + 1;

        return $"pat-{nextNumber:D5}";
    }

    private static int ParsePatientCodeNumber(string code)
    {
        const string prefix = "pat-";
        if (!code.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            return 0;

        var numericPart = code[prefix.Length..];
        return int.TryParse(numericPart, out var number) ? number : 0;
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
