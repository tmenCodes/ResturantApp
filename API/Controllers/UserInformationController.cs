using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Resturant.Business.Dtos;
using Resturant.Business.Services;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserInformationController : Controller
{
    private readonly IUserProfileService _userProfileService;
    private readonly ILogger<UserInformationController> _logger;

    public UserInformationController(IUserProfileService userProfileService, ILogger<UserInformationController> logger)
    {
        _userProfileService = userProfileService;
        _logger = logger;
    }

    private string? CurrentUserName => User.Identity?.Name;

    [Authorize]
    [HttpPost("AddUserPaymentInformation")]
    public async Task<ActionResult> AddUserPaymentInformation(UserPaymentInformationDto userPaymentInformationdto)
    {
        if (CurrentUserName is not { } userName)
        {
            return BadRequest();
        }

        _logger.LogInformation("User {UserName} has logged in.", userName);
        return Ok(await _userProfileService.AddPaymentMethodAsync(userName, userPaymentInformationdto));
    }

    [Authorize]
    [HttpGet("GetUserPaymentInformation")]
    public async Task<IActionResult> GetUserPaymentInformation()
    {
        if (CurrentUserName is not { } userName)
        {
            return BadRequest();
        }

        return Ok(await _userProfileService.GetPaymentMethodAsync(userName));
    }

    [Authorize]
    [HttpGet("UserHistoryPayments")]
    public async Task<IActionResult> GetUserHistoryPayments()
    {
        return Ok(await _userProfileService.GetRecentOrderTotalsAsync(CurrentUserName ?? string.Empty));
    }

    [Authorize]
    [HttpGet("UserHistoryMenu")]
    public async Task<IActionResult> UserHistoryMenu()
    {
        return Ok(await _userProfileService.GetOrderedItemsAsync(CurrentUserName ?? string.Empty));
    }

    [HttpPut("UpdateUserPaymentInformation")]
    public async Task<IActionResult> UpdateUserPaymentInformation(UserPaymentInformationDto userPaymentInformationdto)
    {
        if (CurrentUserName is not { } userName)
        {
            return BadRequest();
        }

        var updated = await _userProfileService.UpdatePaymentMethodAsync(userName, userPaymentInformationdto);
        return updated is null ? BadRequest() : Ok(updated);
    }
}
