namespace Resturant.Data.Repositories;

/// <summary>An item a user ordered, paired with the checkout that paid for it.</summary>
public record CheckedOutOrderItem(DateTime CheckoutTime, Guid ShoppingCartIdentity, string Name, decimal TotalItemPrice);
