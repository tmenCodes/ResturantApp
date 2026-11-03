using Resturant.Application.Dtos;
using Resturant.Domain.Ordering;
namespace Resturant.Application.Mappings;

internal static class CartMappings
{
    public static CartOrderItemDto ToCartDto(this OrderItem orderItem) => new()
    {
        Id = orderItem.Id,
        Name = orderItem.Name,
        Quantity = orderItem.Quantity,
        TotalItemPrice = orderItem.TotalItemPrice,
        Status = orderItem.Status,
        ShoppingCartIdentity = orderItem.ShoppingCartIdentity,
        Options = orderItem.Options.Select(x => new CartOrderItemOptionDto
        {
            Id = x.Id,
            Name = x.Name,
            Price = x.Price,
            OrderItemId = x.OrderItemId
        }).ToList()
    };
}
