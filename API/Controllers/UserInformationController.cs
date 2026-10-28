using Azure;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Resturant.Domain.Entity;
using Resturant.Infrastructure.Identity;
using Resturant.Infrastructure.Context;
using Resturant.Application.DTO;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]

public class UserInformationController : Controller
{
    private readonly RestaurantContext _context;
    private readonly ILogger<UserInformationController> _logger;
    private readonly UserManager<WebUser> _userManager;

    public UserInformationController(RestaurantContext context, ILogger<UserInformationController> logger,
        UserManager<WebUser> userManager)
    {
        _context = context;
        _logger = logger;
        _userManager = userManager;
    }


    [Authorize]
    [HttpPost("AddUserPaymentInformation")]
    public async Task<ActionResult> AddUserPaymentInformation(UserPaymentInformationDto userPaymentInformationdto)
    {

        var user = await _userManager.GetUserAsync(User);
        if (user is not null)
        {
            _logger.LogInformation($"User {user.UserName} has logged in.");
            
            SavedPaymentMethod userpaymentinfo = new SavedPaymentMethod
            {
                Credit = userPaymentInformationdto.CardType,
                NameonCard = userPaymentInformationdto.NameonCard,
                CreditCardNumber = userPaymentInformationdto.CreditCardNumber,
                Expiration = userPaymentInformationdto.Expiration,
                CVV = userPaymentInformationdto.CVV,
                UserName = user.UserName,
            };
            await _context.SavedPaymentMethods.AddAsync(userpaymentinfo);
            await _context.SaveChangesAsync();
            return Ok(userpaymentinfo);
        }

        return BadRequest();
    }


   [Authorize]
    [HttpGet("GetUserPaymentInformation")]
    public async Task<IActionResult> GetUserPaymentInformation()
    {
        var user = await _userManager.GetUserAsync(User);
       // var user = HttpContext.User.Identity.Name;
       if (user is not null)
       {
           var userPaymentInformation = await _context.SavedPaymentMethods.OrderBy(x => x.Id)
               .FirstOrDefaultAsync(x => x.UserName == user!.UserName);
           return Ok(userPaymentInformation);
       }
      
        return BadRequest();
       
    }

    [Authorize]
    [HttpGet("UserHistoryPayments")]
    public async Task<IActionResult> GetUserHistoryPayments()
    {
        var user = HttpContext?.User?.Identity?.Name;

        //getting the payment information
        var User = await _context.Checkouts.Where(name => name.UserProfileName == user)
            .ToListAsync();

        //getting the actual car information
        var shoppingCartItems = await _context.ShoppingCarts.ToListAsync();


        // //join to the two tables above 
        var joining = from x in User
            join temp in shoppingCartItems on x.ShoppingCartIdentity equals temp.Identity

            //   select new UserHistoryDTO() { UserName =  x.UserProfileName, OrderId = temp.Identity.ToString(),  TotalPrice  = temp.TotalPrice, Date = temp.Created.ToString("d"), };
            select new { TotalPrice = temp.TotalPrice, Date = temp.Created.ToString("d") };
        //can do another join here if needed !!!!
        //joing based on guid ida...

        var orderbydate = joining.OrderBy(x => x.Date).Take(5);
        return Ok(orderbydate);
    }


    //user mergemap on frontend if needed 
    [Authorize]
    [HttpGet("UserHistoryMenu")]
    public async Task<IActionResult> UserHistoryMenu()
    {
        var user = HttpContext?.User?.Identity?.Name;

        //getting the payment information
        var User = await _context.Checkouts.Where(name => name.UserProfileName == user).Distinct()
            .ToListAsync();

        //get the guid, from user then jst query 
        var MenuItems = await _context.OrderItem.ToListAsync();
        //  var guidy = await _context.MenuItems.Where(x => x.ShoppingCart == Guids).ToListAsync();


        // //join to the two tables above 
        var joining =
            from customer in User
            join items in MenuItems on customer.ShoppingCartIdentity equals items.ShoppingCartIdentity
            select new UserPaymentMenuItems()
            {
                Date = customer.CheckoutTime,
                Identity = customer.ShoppingCartIdentity.ToString(),
                UserProfileName = customer.UserProfileName,
                MenuItems = new List<MenuItemsBro>()
                {
                    new MenuItemsBro()
                    {
                        Date = customer.CheckoutTime.ToString(), Identity = customer.ShoppingCartIdentity.ToString(),
                        Name = items.Name, Price = items.TotalItemPrice.ToString("0.00")
                    }
                },

            };

        //flattened the array
        //array within an array 
        var combineall = joining.SelectMany(x => x.MenuItems);
        //.GroupBy((x => x.Identity));

        //foreach loop here 

        //just make a list of items first 
        return Ok(combineall);

    }

    class UserPaymentMenuItems
    {
        public DateTime Date { get; set; }
        public string Identity { get; set; }
        public string UserProfileName { get; set; }
        public List<MenuItemsBro> MenuItems { get; set; }

    }

    public class MenuItemsBro
    {
        public string Date { get; set; }
        public string Identity { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
    }


    
    [HttpPut("UpdateUserPaymentInformation")]
    public async Task<IActionResult> UpdateUserPaymentInformation(UserPaymentInformationDto userPaymentInformationdto)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user is not null)
        {
            var userPaymentInformation = await _context.SavedPaymentMethods.OrderBy(users => users.Id)
                .FirstOrDefaultAsync(x => x.UserName == user.UserName);
            if (userPaymentInformation is not null)
            {
                userPaymentInformation.Credit = userPaymentInformationdto.CardType;
                userPaymentInformation.NameonCard = userPaymentInformationdto.NameonCard;
                userPaymentInformation.CreditCardNumber = userPaymentInformationdto.CreditCardNumber;
                userPaymentInformation.Expiration = userPaymentInformationdto.Expiration;
                userPaymentInformation.CVV = userPaymentInformationdto.CVV;
                
                await _context.SaveChangesAsync();
                return Ok(userPaymentInformation);
            }
            return BadRequest();
        }
        return BadRequest();
    }



}