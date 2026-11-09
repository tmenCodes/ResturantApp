namespace API.Features.Cart.Shared;

/// <summary>How cart endpoints read the cart identity the frontend sends as a query string.</summary>
public static class CartIdentity
{
    /// <summary>The identity queried when the frontend sends none; no real cart uses it.</summary>
    public const string Default = "3fa85f64-5717-4562-b3fc-2c963f66afa6";

    /// <summary>Parses the identity, treating a missing or malformed value as <see cref="Guid.Empty"/>, which matches no cart.</summary>
    public static Guid ParseOrEmpty(string? value) => Guid.TryParse(value, out var identity) ? identity : Guid.Empty;
}
