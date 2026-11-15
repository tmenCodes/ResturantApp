using Resturant.Domain.Cart;
using Resturant.Domain.Ordering;
using Resturant.Domain.Payments;
using Resturant.Domain.Repositories;

namespace Domain.Tests.Fakes;

/// <summary>In-memory <see cref="ICheckoutRepository"/> for domain service tests.</summary>
public class FakeCheckoutRepository : ICheckoutRepository
{
    public List<CheckoutInformation> Checkouts { get; } = new();

    public Task<List<CheckoutInformation>> GetAllAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(Checkouts.ToList());

    public Task<bool> ExistsForCartAsync(Guid shoppingCartIdentity, CancellationToken cancellationToken = default) =>
        Task.FromResult(Checkouts.Any(x => x.ShoppingCartIdentity == shoppingCartIdentity));

    public Task AddAsync(CheckoutInformation checkout, CancellationToken cancellationToken = default)
    {
        Checkouts.Add(checkout);
        return Task.CompletedTask;
    }
}
