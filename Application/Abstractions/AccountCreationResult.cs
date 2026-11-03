using Resturant.Application.Dtos;

namespace Resturant.Application.Abstractions;

/// <summary>The outcome of asking the identity provider to create a user. <see cref="Errors"/> is empty on success.</summary>
public record AccountCreationResult(bool Succeeded, IReadOnlyList<AccountError> Errors);
