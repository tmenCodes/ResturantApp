namespace Resturant.Business.Dtos;

/// <summary>Why a checkout attempt succeeded or was rejected.</summary>
public enum CheckoutStatus
{
    Created,
    CartAlreadyCheckedOut,
    CartNotFound
}

/// <summary>The outcome of a checkout attempt. <see cref="Checkout"/> is set only when <see cref="Status"/> is <see cref="CheckoutStatus.Created"/>.</summary>
public record CheckoutResult(CheckoutStatus Status, CheckoutDto? Checkout = null);
