using Resturant.Business.Dtos;
using Resturant.Business.Services;
using Resturant.Data.Context;
using Resturant.Data.Entities;
using Resturant.Data.Repositories;

namespace Business.Tests;

public class UserProfileServiceTests
{
    private static UserProfileService CreateService(RestaurantContext context) =>
        new(new UserProfileRepository(context));

    private static UserPaymentInformationDto Payment(string nameOnCard) =>
        new("Visa", nameOnCard, "4111", "12/30", "123");

    private static async Task<Guid> AddCheckedOutCartAsync(RestaurantContext context, string userName, DateTime created, decimal total)
    {
        var identity = Guid.NewGuid();
        context.ShoppingCarts.Add(new ShoppingCart { Identity = identity, Created = created, TotalPrice = total });
        context.Checkouts.Add(new Checkout { UserProfileName = userName, ShoppingCartIdentity = identity, CheckoutTime = created, Paid = true });
        await context.SaveChangesAsync();
        return identity;
    }

    [Fact]
    public async Task AddPaymentMethodAsync_SavesPaymentMethodForUser()
    {
        await using var context = TestDatabase.Create();

        var result = await CreateService(context).AddPaymentMethodAsync("ada", Payment("Ada Lovelace"));

        var saved = Assert.Single(context.SavedPaymentMethods);
        Assert.Equal(saved.Id, result.Id);
        Assert.Equal("ada", saved.UserName);
        Assert.Equal("Visa", saved.Credit);
        Assert.Equal("Ada Lovelace", result.NameonCard);
    }

    [Fact]
    public async Task GetPaymentMethodAsync_ReturnsOnlyThatUsersFirstPaymentMethod()
    {
        await using var context = TestDatabase.Create();
        var service = CreateService(context);
        await service.AddPaymentMethodAsync("grace", Payment("Grace Hopper"));
        await service.AddPaymentMethodAsync("ada", Payment("First"));
        await service.AddPaymentMethodAsync("ada", Payment("Second"));

        var result = await service.GetPaymentMethodAsync("ada");

        Assert.Equal("First", result!.NameonCard);
    }

    [Fact]
    public async Task GetPaymentMethodAsync_NoPaymentMethod_ReturnsNull()
    {
        await using var context = TestDatabase.Create();

        Assert.Null(await CreateService(context).GetPaymentMethodAsync("ada"));
    }

    [Fact]
    public async Task UpdatePaymentMethodAsync_ExistingPaymentMethod_OverwritesIt()
    {
        await using var context = TestDatabase.Create();
        var service = CreateService(context);
        await service.AddPaymentMethodAsync("ada", Payment("Old Name"));

        var result = await service.UpdatePaymentMethodAsync("ada", Payment("New Name"));

        Assert.Equal("New Name", result!.NameonCard);
        Assert.Equal("New Name", Assert.Single(context.SavedPaymentMethods).NameonCard);
    }

    [Fact]
    public async Task UpdatePaymentMethodAsync_NoPaymentMethod_ReturnsNullAndSavesNothing()
    {
        await using var context = TestDatabase.Create();

        var result = await CreateService(context).UpdatePaymentMethodAsync("ada", Payment("Ada Lovelace"));

        Assert.Null(result);
        Assert.Empty(context.SavedPaymentMethods);
    }

    [Fact]
    public async Task GetRecentOrderTotalsAsync_ReturnsAtMostFiveOfTheUsersOrders()
    {
        await using var context = TestDatabase.Create();
        for (var day = 1; day <= 6; day++)
        {
            await AddCheckedOutCartAsync(context, "ada", new DateTime(2026, 1, day), day);
        }
        await AddCheckedOutCartAsync(context, "grace", new DateTime(2026, 1, 1), 99m);

        var result = await CreateService(context).GetRecentOrderTotalsAsync("ada");

        Assert.Equal(5, result.Count);
        Assert.DoesNotContain(result, history => history.TotalPrice == 99m);
        Assert.Equal(new DateTime(2026, 1, 1).ToString("d"), result[0].Date);
    }

    [Fact]
    public async Task GetOrderedItemsAsync_ReturnsFormattedItemsFromTheUsersCheckedOutCarts()
    {
        await using var context = TestDatabase.Create();
        var checkoutTime = new DateTime(2026, 10, 28, 12, 0, 0);
        var cart = await AddCheckedOutCartAsync(context, "ada", checkoutTime, 10m);
        var otherCart = await AddCheckedOutCartAsync(context, "grace", checkoutTime, 10m);
        context.OrderItem.Add(new OrderItem { Name = "Papaya Salad", TotalItemPrice = 8.5m, ShoppingCartIdentity = cart });
        context.OrderItem.Add(new OrderItem { Name = "Tofu Stir Fry", TotalItemPrice = 12.95m, ShoppingCartIdentity = otherCart });
        await context.SaveChangesAsync();

        var result = await CreateService(context).GetOrderedItemsAsync("ada");

        var item = Assert.Single(result);
        Assert.Equal(new UserOrderedItemDto(checkoutTime.ToString(), cart.ToString(), "Papaya Salad", "8.50"), item);
    }
}
