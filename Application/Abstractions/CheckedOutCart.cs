namespace Resturant.Application.Abstractions;

/// <summary>Read model: when a checked-out cart was created and what it cost in total.</summary>
public record CheckedOutCart(DateTime Created, decimal TotalPrice);
