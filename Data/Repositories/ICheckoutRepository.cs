using Resturant.Data.Entities;

namespace Resturant.Data.Repositories;

/// <summary>Reads and writes checkouts (paid shopping carts).</summary>
public interface ICheckoutRepository
{
    /// <summary>Returns every checkout.</summary>
    Task<List<Checkout>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Returns true when the shopping cart with this identity has already been checked out.</summary>
    Task<bool> ExistsForCartAsync(Guid shoppingCartIdentity, CancellationToken cancellationToken = default);

    /// <summary>Saves a new checkout and assigns its <see cref="Checkout.Id"/>.</summary>
    Task AddAsync(Checkout checkout, CancellationToken cancellationToken = default);
}
