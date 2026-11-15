using API.Common;
using Microsoft.EntityFrameworkCore;
using Resturant.Infrastructure.Persistence;

namespace API.Features.Cart.GetCartTotals;

/// <summary>Reads a cart's totals.</summary>
public class GetCartTotalsHandler : IHandler
{
    private readonly RestaurantContext _context;

    public GetCartTotalsHandler(RestaurantContext context)
    {
        _context = context;
    }

    /// <summary>Returns the cart's subtotal, tax and total formatted to two decimals, or null when the cart does not exist.</summary>
    public async Task<CartTotalsDto?> HandleAsync(string cartIdentity)
    {
        if (!Guid.TryParse(cartIdentity, out var identity))
        {
            return null;
        }

        var totals = await _context.ShoppingCarts
            .Where(x => x.Identity == identity)
            .Select(x => new { x.SubTotal, x.TaxAmount, x.TotalPrice })
            .FirstOrDefaultAsync();
        if (totals is null)
        {
            return null;
        }

        return new CartTotalsDto
        {
            SubTotal = totals.SubTotal.ToString("0.00"),
            TaxAmount = totals.TaxAmount.ToString("0.00"),
            TotalPrice = totals.TotalPrice.ToString("0.00")
        };
    }
}
