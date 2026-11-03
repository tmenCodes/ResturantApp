namespace Resturant.Domain.Services.Ordering;

/// <summary>Whether a shopping cart may be checked out, and why not.</summary>
public enum CheckoutEligibilityResult
{
    Eligible,
    CartNotFound,
    CartAlreadyCheckedOut
}
