using Resturant.Domain.Ordering;
using Resturant.Domain.Repositories;

namespace Domain.Tests.Fakes;

/// <summary>In-memory <see cref="IShoppingCartRepository"/> for domain service tests.</summary>
public class FakeShoppingCartRepository : IShoppingCartRepository
{
    public List<ShoppingCart> Carts { get; } = new();

    public Task<ShoppingCart?> GetCartAsync(Guid cartIdentity) =>
        Task.FromResult(Carts.FirstOrDefault(x => x.Identity == cartIdentity));

    public Task<int> SaveChangesAsync() => Task.FromResult(0);

    public Task<List<OrderItem>> GetAllOrderItemsAsync() =>
        Task.FromResult(Carts.SelectMany(x => x.OrderItems).ToList());

    public Task<List<OrderItem>> GetCartOrderItemsAsync(Guid cartIdentity) =>
        Task.FromResult(Carts.Where(x => x.Identity == cartIdentity).SelectMany(x => x.OrderItems).ToList());

    public Task AddCartAsync(ShoppingCart cart)
    {
        Carts.Add(cart);
        return Task.CompletedTask;
    }
}
