using Resturant.Domain.Menu;
using Resturant.Domain.Ordering;

namespace Domain.Tests;

public class ShoppingCartTests
{
    private static readonly DateTime Now = new(2026, 10, 28, 12, 0, 0, DateTimeKind.Utc);
    private static readonly MenuItem TofuStirFry = new() { Name = "Tofu Stir Fry", Description = "Tofu", BasePrice = 10m };

    private static OrderItem Item(int id, int quantity)
    {
        var item = OrderItem.Create(TofuStirFry, [], quantity);
        item.Id = id;
        return item;
    }

    [Fact]
    public void Create_StartsEmptyWithDefaultTaxRate()
    {
        var identity = Guid.NewGuid();

        var cart = ShoppingCart.Create(identity, Now);

        Assert.Equal(identity, cart.Identity);
        Assert.Equal(Now, cart.Created);
        Assert.Equal(ShoppingCart.DefaultTaxRate, cart.TaxRate);
        Assert.Empty(cart.OrderItems);
        Assert.Equal(0m, cart.TotalPrice);
    }

    [Fact]
    public void AddItem_RecalculatesSubTotalTaxAndTotal()
    {
        var cart = ShoppingCart.Create(Guid.NewGuid(), Now);

        cart.AddItem(Item(1, 1));
        cart.AddItem(Item(2, 2));

        Assert.Equal(30m, cart.SubTotal);
        Assert.Equal(30m * ShoppingCart.DefaultTaxRate, cart.TaxAmount);
        Assert.Equal(30m + 30m * ShoppingCart.DefaultTaxRate, cart.TotalPrice);
    }

    [Fact]
    public void RemoveItem_KnownItem_RemovesItAndRecalculatesTotals()
    {
        var cart = ShoppingCart.Create(Guid.NewGuid(), Now);
        cart.AddItem(Item(1, 1));
        cart.AddItem(Item(2, 2));

        Assert.True(cart.RemoveItem(1));

        Assert.Single(cart.OrderItems);
        Assert.Equal(20m, cart.SubTotal);
        Assert.Equal(20m + 20m * ShoppingCart.DefaultTaxRate, cart.TotalPrice);
    }

    [Fact]
    public void RemoveItem_UnknownItem_ReturnsFalseAndKeepsTotals()
    {
        var cart = ShoppingCart.Create(Guid.NewGuid(), Now);
        cart.AddItem(Item(1, 1));

        Assert.False(cart.RemoveItem(99));

        Assert.Single(cart.OrderItems);
        Assert.Equal(10m, cart.SubTotal);
    }
}
