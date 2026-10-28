using Microsoft.AspNetCore.Mvc;
using Resturant.Business.Dtos;
using Resturant.Business.Services;
using API.Contracts;

namespace API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : Controller
    {

        private readonly ICheckoutService _checkoutService;
        private readonly ILogger _logger;

        public PaymentController(ICheckoutService checkoutService, ILogger<PaymentController> logger)
        {
            _checkoutService = checkoutService ?? throw new ArgumentNullException(nameof(checkoutService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        [HttpGet("GetAllOrders")]
        public async Task<ActionResult<IEnumerable<CheckoutDto>>> GetAllOrders(CancellationToken cancellationToken)
        {
            var orders = await _checkoutService.GetAllAsync(cancellationToken);

            if (orders.Any())
            {
                return Ok(orders);
            }
            return BadRequest("No orders were found.");
        }


        //TODO work on this controller in the near future-possibly use GroupBy
        [HttpGet("GetAllOrdersWithMenu")]
        public ActionResult<ViewsDto> GetAllOrdersWith([FromQuery] string orderGuid = "3fa85f64-5717-4562-b3fc-2c963f66afa6")
        {
            return Ok("completed for now");
        }


        [HttpPost("PaymentInformation")]
        public async Task<ActionResult<CheckoutDto>> PostOrderInformation(
            CustomerInformationDTO customerInformationDto, CancellationToken cancellationToken)
        {
            _logger.LogCritical(customerInformationDto.CartsIdentity);

            var result = await _checkoutService.CreateAsync(customerInformationDto, cancellationToken);

            return result.Status switch
            {
                CheckoutStatus.Created => Ok(result.Checkout),
                CheckoutStatus.CartAlreadyCheckedOut => BadRequest("Another pre-existing user has that guid"),
                _ => BadRequest("Invalid guid")
            };
        }
    }
    }
