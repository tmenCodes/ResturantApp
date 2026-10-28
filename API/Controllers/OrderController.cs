
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Hangfire.Dashboard;
using Resturant.Data.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using Resturant.Business.Dtos;
using Resturant.Data.Repositories;
using Resturant.Data.Context;
using MenuItem = Resturant.Data.Entities.MenuItem;


namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : Controller
    {
        //private readonly IMediator _mediator;
        private readonly  RestaurantContext _context;
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly ILogger<OrderController> _logger;
        
        
        public OrderController(RestaurantContext context,  IShoppingCartRepository shoppingCartRepository , ILogger<OrderController> logger)
        {
            //   _mediator = mediatR ?? throw  new ArgumentNullException(nameof(mediatR));
            _context = context; 
           _shoppingCartRepository = shoppingCartRepository;
           _logger = logger;
        }

        
        [HttpGet("GetTotalPrice")]
        public async Task<IActionResult> GetShoppingCartItemsByGuid(string guid)
        { 
       
           ShoppingCart? shoppingCartItems = await _shoppingCartRepository.ReturnCartItemsByGuidAsync(guid);
           
           if(shoppingCartItems == null) return NotFound();
           
           ShoppingCartItemDTO shoppingcartItemDto = new ShoppingCartItemDTO();
           shoppingcartItemDto.SubTotal = shoppingCartItems.SubTotal.ToString("0.00");
           shoppingcartItemDto.TaxAmount = shoppingCartItems.TaxAmount.ToString("0.00");
           shoppingcartItemDto.TotalPrice = shoppingCartItems.TotalPrice.ToString("0.00");
           return Ok(shoppingcartItemDto);
           
        }
        
        
         [HttpGet("GetAllTempItems")]
         public async Task<ActionResult<IEnumerable<MenuDTO>>> ShoppingCartItems()
         {
             var menuDtos = (await _shoppingCartRepository.ReturnMenuItemsListAsync())
                 .Select(x => new MenuDTO() { Id = x.Id, Name = x.Name, Price = x.TotalItemPrice});
             return menuDtos.Any() ? Ok(menuDtos) : NotFound("no value was found");
         }


         [HttpGet("GetMenuItemByGuid")]
         public async Task<ActionResult<IEnumerable<MenuDTO>>> ShoppingCartItemByGuid(
             [FromQuery] string guidId = "3fa85f64-5717-4562-b3fc-2c963f66afa6")
         {
             Guid.TryParse(guidId, out var guidIdValue);
             
             
             
             var menuDto = (await _shoppingCartRepository.ReturnMenuItemListByGuid(guidIdValue.ToString()))
              //   .Select(x => new MenuDTO()
                //     { Id = x.Id, Name = x.Name, Price = x.TotalItemPrice})
                 .ToList();

             return Ok(menuDto);
         }
         
         [HttpGet("GetMenuItemByGuidSize")]
         public async Task<ActionResult<int>> ShoppingCartItemByGuidSize(
             [FromQuery] string guidId = "3fa85f64-5717-4562-b3fc-2c963f66afa6")
         {
             Guid.TryParse(guidId, out var menuGuidValue);

             var menuDto = (await _shoppingCartRepository.ReturnMenuItemListByGuid(menuGuidValue.ToString()))
                 .Select(x => new MenuDTO()
                     { Id = x.Id, Name = x.Name, Price = x.TotalItemPrice})
                 .ToList();
   

             int menuDTOSize = menuDto.Count;
             
           return Ok(menuDTOSize);
         }
         
         
         [ProducesResponseType(200)]
         [HttpDelete("DeleteMenuItem")]
         public async Task<IActionResult> RemoveMenuItemAndUpdatePrice(int id, Guid guidId)
         {
            var menuItem = await _shoppingCartRepository.FindByPrimaryKey(id); 
            if (menuItem is not null)
            { 
                _context.OrderItem.Remove(menuItem);
                await _shoppingCartRepository.SaveCartItemsAsync();
                //update subtotal
                var subTotalMenuPrice = _shoppingCartRepository.SubTotalMenuPrice(guidId); 
                var shoppingCartItems = await _shoppingCartRepository.ReturnCartItemsByGuidAsync(guidId.ToString());
               
                //update prices
                shoppingCartItems!.SubTotal = subTotalMenuPrice;
                shoppingCartItems.TaxAmount = shoppingCartItems.SubTotal * shoppingCartItems.TaxRate;
                shoppingCartItems.TotalPrice = shoppingCartItems.SubTotal + shoppingCartItems.TaxAmount;
                
                await _context.SaveChangesAsync();
    
                return Ok(); 
            }
            return NotFound("No menu item was found");
            
         }

         [HttpGet("GetMenuItems")]
         public async Task<IActionResult> GetMenuItems()
         {
             var x = from menuitems in _context.MenuItems
                 select new { Id = menuitems.Id , MenuItemName = menuitems.Name, MenuItemBasePrice = menuitems.BasePrice.ToString("0.00") , MenuItemDescription = menuitems.Description, MenuItemImageUrl = menuitems.ImageUrl };
             return Ok(x);
         }
         
         [HttpGet("GetMenuItemOptions")]
         public async Task<IActionResult> GetMenuOptions()
         {
             return Ok(await _context.MenuItemOptions.ToListAsync()); 
         }
         
         
         [HttpPost("ShoppingCartItems")]
         public async Task<ActionResult<OrderItemDto>> AddMenuItems(OrderItemDto orderItemDto)
         {
             //TODO put taxrate in the menuItem domain model method
             const decimal taxRate = 0.06875m;
             decimal totalMenuItemOptionsPrice = 0m; 
             ShoppingCart? shoppingCartItems = await _shoppingCartRepository.ReturnCartItemsByGuidAsync(orderItemDto.GuidId.ToString());
             
        
           if (shoppingCartItems is null)
           {
               var menuItem = await _context.MenuItems.SingleOrDefaultAsync(value => value.Name == orderItemDto.Name);
               
                   var orderItem = new OrderItem(); 
                   orderItem.Name = menuItem.Name!;
                   orderItem.Status = "start";
                   //put valiation elsewhere 
                   if (orderItemDto.Quantity > 0)
                   {
                       orderItem.Quantity = orderItemDto.Quantity;
                   }
                   else
                   {
                       throw new Exception("value must be greater than 0");
                   }
                   
             
                   foreach (var menuitem in orderItemDto.Options)
                   {
              
                       MenuItemOption? menuItemOptions = new(); 
                       menuItemOptions = await _context.MenuItemOptions.FirstOrDefaultAsync(value => value.Name == menuitem!);
                       
                       if (menuItemOptions != null)
                       {
                           var orderItemOptions1 = new OrderItemOption();

                           orderItemOptions1.Name = menuItemOptions.Name;
                           orderItemOptions1.Price = menuItemOptions.Price;

                           orderItem.Options.Add(orderItemOptions1);

                           totalMenuItemOptionsPrice += menuItemOptions.Price;
                           _logger.LogInformation("the total menuITem prices" + totalMenuItemOptionsPrice);
                       }
                    
                   } 

                   orderItem.TotalItemPrice = (menuItem!.BasePrice + totalMenuItemOptionsPrice) * orderItemDto.Quantity;
                  
                   //TODO put this calculation elsewhere
                   var shoppingcartitems = new ShoppingCart();
                   shoppingcartitems.Identity = orderItemDto.GuidId;
                   shoppingcartitems.Created = DateTime.UtcNow;
                   shoppingcartitems.SubTotal = orderItem.TotalItemPrice; 
                   shoppingcartitems.TaxAmount = orderItem.TotalItemPrice * taxRate;
                   shoppingcartitems.TotalPrice = orderItem.TotalItemPrice + shoppingcartitems.TaxAmount;
                   
                   shoppingcartitems.OrderItems.Add(orderItem);
                   await _shoppingCartRepository.AddShoppingCartItem(shoppingcartitems);
                   await _shoppingCartRepository.SaveCartItemsAsync();
                   return Ok(new { Name = orderItemDto.Name!, Price = menuItem.BasePrice });
      
           }
           //continue to add items to existing shopping cart
           else
           {
               string name = orderItemDto.Name!;

               var menuItem = await _context.MenuItems.FirstOrDefaultAsync(value => value.Name == orderItemDto.Name); 
     
               var orderItem = new OrderItem();
               orderItem.Name = name;
               orderItem.Status = "start";
               orderItem.TotalItemPrice = menuItem!.BasePrice;
               if (orderItemDto.Quantity > 0)
               {
                   orderItem.Quantity = orderItemDto.Quantity;
               }
               else
               {
                   throw new Exception("value must be greater than 0");
               }
               
   
               //if there is a valid menu options 
               foreach (var x in orderItemDto.Options)
               {
                   var menuItemOptions = await _context.MenuItemOptions.FirstOrDefaultAsync(value => value.Name == x!);

                   if (menuItemOptions != null)
                   {
                       var orderItemOptions1 = new OrderItemOption(); // create new object each loop

                       orderItemOptions1.Name = menuItemOptions.Name;
                       orderItemOptions1.Price = menuItemOptions.Price;

                       orderItem.Options.Add(orderItemOptions1);

                       totalMenuItemOptionsPrice += menuItemOptions.Price;
                       _logger.LogCritical(totalMenuItemOptionsPrice.ToString());
                   }
                   
               } 
               
               orderItem.TotalItemPrice = (menuItem!.BasePrice + totalMenuItemOptionsPrice) * orderItemDto.Quantity;
               
               shoppingCartItems.OrderItems.Add(orderItem);
               
               await CalculateTotalAmountShoppingCart(orderItem.TotalItemPrice); 
               
               async Task CalculateTotalAmountShoppingCart(decimal newItemTotalPrice)
               {     
                   
                   var currentTotalMenuPrice = _shoppingCartRepository.SubTotalMenuPrice(orderItemDto.GuidId);
                   _logger.LogCritical($"Current total menu price: {currentTotalMenuPrice}");
                   var totalmenuprice = currentTotalMenuPrice + newItemTotalPrice;
               
                   shoppingCartItems.SubTotal = totalmenuprice; 
                   shoppingCartItems.TaxAmount =  totalmenuprice * shoppingCartItems.TaxRate;
                   shoppingCartItems.TotalPrice = totalmenuprice + shoppingCartItems.TaxAmount;
                   await _shoppingCartRepository.SaveCartItemsAsync();
               }
               
             return Ok(new { Name = name, Price =  orderItem.TotalItemPrice });
           }
       
         }

         [HttpGet("ImagePath")]
         public async Task<IActionResult> GetImagePath()
         {
             var urlPath = _context.MenuItems.Select(value => value.ImageUrl).ToList(); 
            return Ok(urlPath);
         }
         
         
         
    }
}
