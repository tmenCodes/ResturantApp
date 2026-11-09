namespace API.Features.Account.CreateUser;

/// <summary>A reason the account was rejected, sent as <c>{ code, description }</c>.</summary>
public record AccountErrorDto(string Code, string Description);
