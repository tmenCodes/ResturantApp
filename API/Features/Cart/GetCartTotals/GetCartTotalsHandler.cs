using API.Common;
using Resturant.Domain.Repositories;

namespace API.Features.Cart.GetCartTotals;

/// <summary>Reads a cart's totals.</summary>
public class GetCartTotalsHandler : IHandler
{
    private readonly IShoppingCartRepository _shoppingCartRepository;

    public GetCartTotalsHandler(IShoppingCartRepository shoppingCartRepository)
    {
        _shoppingCartRepository = shoppingCartRepository;
    }

    /// <summary>Returns the cart's subtotal, tax and total formatted to two decimals, or null when the cart does not exist.</summary>
    public async Task<CartTotalsDto?> HandleAsync(string cartIdentity)
    {
        if (!Guid.TryParse(cartIdentity, out var identity))
        {
            return null;
        }

        var cart = await _shoppingCartRepository.GetCartAsync(identity);
        if (cart is null)
        {
            return null;
        }

        return new CartTotalsDto
        {
            SubTotal = cart.SubTotal.ToString("0.00"),
            TaxAmount = cart.TaxAmount.ToString("0.00"),
            TotalPrice = cart.TotalPrice.ToString("0.00")
        };
    }
}
