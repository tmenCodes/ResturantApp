namespace Resturant.Application.Abstractions;

/// <summary>A registered user as seen by the application core, independent of the identity provider.</summary>
public record UserAccount(string Id, string? UserName, string? Email, string? FullName);
