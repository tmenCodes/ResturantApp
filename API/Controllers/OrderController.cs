using Microsoft.AspNetCore.Mvc;
using Resturant.Business.Dtos;
using Resturant.Business.Services;


namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IMenuService _menuService;


        public OrderController(ICartService cartService, IMenuService menuService)
        {
            _cartService = cartService;
            _menuService = menuService;
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

         [HttpGet("GetMenuItems")]
         public async Task<IActionResult> GetMenuItems(CancellationToken cancellationToken)
         {
             return Ok(await _menuService.GetMenuItemsAsync(cancellationToken));
         }

         [HttpGet("GetMenuItemOptions")]
         public async Task<IActionResult> GetMenuOptions(CancellationToken cancellationToken)
         {
             return Ok(await _menuService.GetMenuItemOptionsAsync(cancellationToken));
         }


         [HttpPost("ShoppingCartItems")]
         public async Task<ActionResult<AddToCartResultDto>> AddMenuItems(OrderItemDto orderItemDto, CancellationToken cancellationToken)
         {
             return Ok(await _cartService.AddItemAsync(orderItemDto, cancellationToken));
         }

         [HttpGet("ImagePath")]
         public async Task<IActionResult> GetImagePath(CancellationToken cancellationToken)
         {
            return Ok(await _menuService.GetImageUrlsAsync(cancellationToken));
         }



    }
}
