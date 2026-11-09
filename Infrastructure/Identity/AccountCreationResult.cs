namespace Resturant.Infrastructure.Identity;

/// <summary>The outcome of asking the identity provider to create a user. <see cref="Errors"/> is empty on success.</summary>
public record AccountCreationResult(bool Succeeded, IReadOnlyList<AccountError> Errors);
