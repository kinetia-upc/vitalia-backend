namespace VitaliaBackend.Iam.Interfaces.Rest.Resources;

public record AuthenticatedUserResource(UserResource User, string Token);
