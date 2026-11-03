namespace Resturant.Application.Ports.Out;

/// <summary>An item a user ordered, paired with the checkout that paid for it.</summary>
public record CheckedOutOrderItem(DateTime CheckoutTime, Guid ShoppingCartIdentity, string Name, decimal TotalItemPrice);
