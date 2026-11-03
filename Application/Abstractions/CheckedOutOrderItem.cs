namespace Resturant.Application.Abstractions;

/// <summary>Read model: an item a user ordered, paired with the checkout that paid for it.</summary>
public record CheckedOutOrderItem(DateTime CheckoutTime, Guid ShoppingCartIdentity, string Name, decimal TotalItemPrice);
