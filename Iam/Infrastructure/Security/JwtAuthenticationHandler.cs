using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using VitaliaBackend.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

namespace VitaliaBackend.Iam.Infrastructure.Security;

public class JwtAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IConfiguration configuration,
    AppDbContext context)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
            return AuthenticateResult.NoResult();

        var value = authorizationHeader.ToString();
        if (!value.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return AuthenticateResult.NoResult();

        var token = value[7..].Trim();
        if (!JwtTokenValidator.TryValidate(token, configuration, out var userId, out var role))
            return AuthenticateResult.Fail("Invalid token.");

        var user = await context.Users.AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == userId && item.IsActive);

        if (user is null)
            return AuthenticateResult.Fail("Inactive or unknown user.");

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new("healthcareCenterId", user.HealthcareCenterId)
        };

        var currentRole = string.IsNullOrWhiteSpace(user.Role) ? role : user.Role;
        if (!string.IsNullOrWhiteSpace(currentRole))
            claims.Add(new Claim(ClaimTypes.Role, currentRole));

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }
}
