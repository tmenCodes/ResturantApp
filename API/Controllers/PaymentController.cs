using System.Collections.Immutable;
using Hangfire.Dashboard;
using Hangfire.Logging.LogProviders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Resturant.Infrastructure.Context;
using Resturant.Application.DTO;
using Resturant.Domain.Entity;
using API.Contracts;
using Microsoft.EntityFrameworkCore;
using MenuItem = Resturant.Domain.Entity.MenuItem;

namespace API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : Controller
    {

        private readonly RestaurantContext _context;  
        private readonly ILogger _logger;

        public PaymentController(RestaurantContext context, ILogger<PaymentController> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context)); 
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
 
        [HttpGet("GetAllOrders")]
        public async Task<ActionResult<IEnumerable<CustomerPaymentInformation>>> GetAllOrders()
        {
            //use a dto to get menu items also 
            var orders = await _context.CustomerPaymentInformation.ToListAsync();
            
            if (orders.Any())
            {
                return Ok(orders);
            }
            return BadRequest("No orders were found.");
        }


        
        
        
        //TODO work on this controller in the near future-possibly use GroupBy
        [HttpGet("GetAllOrdersWithMenu")]
        public async Task<ActionResult<ViewsDto>> GetAllOrdersWith([FromQuery] string orderGuid = "3fa85f64-5717-4562-b3fc-2c963f66afa6")
        {
            //possibly use group by 
                var orderList = await _context.CustomerPaymentInformation.ToListAsync();
                var tempList = await _context.ShoppingCartItems.Include("MenuItems").ToListAsync();
                
              
            //join to the two tables above 
               //  var joining = from x in orderList
               //      join temp in tempList on x.ShoppingCartIdentity equals temp.Identity
               //      select new ViewsDto() { Name = x.UserProfileName, OrderId = temp.Identity,  Menus  = temp.UserItems, };
               //
               //
               // var finalValue =  
               //     from x in joining 
               //     where x.OrderId.ToString() == orderGuid
               //     select new { UserName = x.Name, Items = x.Menus.Select(p => p.Name + ", " + p.Price), GuidId = x.OrderId.ToString() }; 
               //
               //  
               //  if (!finalValue.Any())
               //  { 
               //   return NotFound("The order does not exist or the order has not ben created yet");
               //  }
               //  return Ok(finalValue);

               return Ok("completed for now");

        }
        
        




        [HttpPost("PaymentInformation")]
        public async Task<ActionResult<CustomerPaymentInformation>> PostOrderInformation(
            CustomerInformationDTO customerInformationDto)
        {

            //we assume all the guids are unique
            var orders = await _context.CustomerPaymentInformation.FirstOrDefaultAsync(x =>
                x.ShoppingCartIdentity.ToString() == customerInformationDto.CartsIdentity);
            
            if (orders != null)
            {
                return BadRequest("Another pre-existing user has that guid");
            }
            
            var convertGuid = Guid.TryParse(customerInformationDto.CartsIdentity, out var tempGuid);

            _logger.LogCritical(customerInformationDto.CartsIdentity);

            
            //check to make sure the guid must exist in the other table 
            var checkingShoppingCart = await _context.ShoppingCartItems.FirstOrDefaultAsync(x=> x.Identity.ToString() == customerInformationDto.CartsIdentity);
            
            
            if (convertGuid && checkingShoppingCart!= null)
            {
                CustomerPaymentInformation customerPayment = new CustomerPaymentInformation()
                {
                    Credit = customerInformationDto.Credit,
                    NameonCard = customerInformationDto.NameonCard,
                    CreditCardNumber = customerInformationDto.CreditCardNumber,
                    Expiration = customerInformationDto.Expiration,
                    CVV = customerInformationDto.CVV,
                    UserProfileName = customerInformationDto.UserName,
                    ShoppingCartIdentity = tempGuid,
                    CheckoutTime = DateTime.UtcNow,
                    Paid = true
                };

                await _context.CustomerPaymentInformation.AddAsync(customerPayment);
                await _context.SaveChangesAsync();

                return Ok(customerPayment);
            }

            return BadRequest("Invalid guid");
        }
    }
    }