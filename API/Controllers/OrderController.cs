using Microsoft.AspNetCore.Mvc;
using Resturant.Application.Dtos;
using Resturant.Application.Interfaces;
namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : Controller
    {
        private readonly ICartService _cartService;


        public OrderController(ICartService cartService)
        {
            _cartService = cartService;
        }


        [HttpGet("GetTotalPrice")]
        public async Task<IActionResult> GetShoppingCartItemsByGuid(string guid)
        {
           var totals = await _cartService.GetTotalsAsync(guid);
           return totals is null ? NotFound() : Ok(totals);
        }


         [HttpGet("GetAllTempItems")]
         public async Task<ActionResult<IEnumerable<MenuDTO>>> ShoppingCartItems()
         {
             var menuDtos = await _cartService.GetAllOrderItemsAsync();
             return menuDtos.Any() ? Ok(menuDtos) : NotFound("no value was found");
         }


         [HttpGet("GetMenuItemByGuid")]
         public async Task<ActionResult<IEnumerable<CartOrderItemDto>>> ShoppingCartItemByGuid(
             [FromQuery] string guidId = "3fa85f64-5717-4562-b3fc-2c963f66afa6")
         {
             return Ok(await _cartService.GetCartItemsAsync(guidId));
         }

         [HttpGet("GetMenuItemByGuidSize")]
         public async Task<ActionResult<int>> ShoppingCartItemByGuidSize(
             [FromQuery] string guidId = "3fa85f64-5717-4562-b3fc-2c963f66afa6")
         {
           return Ok(await _cartService.GetCartItemCountAsync(guidId));
         }


         [ProducesResponseType(200)]
         [HttpDelete("DeleteMenuItem")]
         public async Task<IActionResult> RemoveMenuItemAndUpdatePrice(int id, Guid guidId)
         {
            var removed = await _cartService.RemoveOrderItemAsync(id, guidId);
            return removed ? Ok() : NotFound("No menu item was found");
         }

         [HttpPost("ShoppingCartItems")]
         public async Task<ActionResult<AddToCartResultDto>> AddMenuItems(OrderItemDto orderItemDto, CancellationToken cancellationToken)
         {
             return Ok(await _cartService.AddItemAsync(orderItemDto, cancellationToken));
         }



    }
}
