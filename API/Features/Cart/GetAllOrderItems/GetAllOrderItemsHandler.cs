using API.Common;
using Resturant.Domain.Repositories;

namespace API.Features.Cart.GetAllOrderItems;

/// <summary>Lists the order items in every cart.</summary>
public class GetAllOrderItemsHandler : IHandler
{
    private readonly IShoppingCartRepository _shoppingCartRepository;

    public GetAllOrderItemsHandler(IShoppingCartRepository shoppingCartRepository)
    {
        _shoppingCartRepository = shoppingCartRepository;
    }

    /// <summary>Returns every order item across all carts.</summary>
    public async Task<IReadOnlyList<OrderItemSummaryDto>> HandleAsync()
    {
        var orderItems = await _shoppingCartRepository.GetAllOrderItemsAsync();
        return orderItems.Select(x => new OrderItemSummaryDto { Id = x.Id, Name = x.Name, Price = x.TotalItemPrice }).ToList();
    }
}
