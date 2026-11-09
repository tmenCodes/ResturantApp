namespace Resturant.Infrastructure.Identity;

/// <summary>A reason the account store rejected an account, serialized as <c>{ code, description }</c>.</summary>
public record AccountError(string Code, string Description);
