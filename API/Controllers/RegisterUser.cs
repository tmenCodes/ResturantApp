using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Resturant.Business.Dtos;
using Resturant.Business.Services;
using API.Contracts;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RegisterUser : Controller
{
    private readonly IAccountService _accountService;
    private readonly ILogger<RegisterUser> _logger;

    public RegisterUser(IAccountService accountService, ILogger<RegisterUser> logger)
    {
        _accountService = accountService;
        _logger = logger;
    }

    [HttpPost("SignOut")]
    public IActionResult SignOut()
    {
        // JWT is stateless — nothing to invalidate server-side. The client discards the token.
        return Ok(new { message = "Signed out." });
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO login)
    {
        if (login?.Email is null || login.Password is null)
            return BadRequest("Email and password are required.");

        var result = await _accountService.LoginAsync(login.Email, login.Password);
        return result.Status switch
        {
            LoginStatus.Succeeded => Ok(new { token = result.Token, message = "Login successful" }),
            LoginStatus.LockedOut => StatusCode(429, "Account is locked. Try again later."),
            _ => Unauthorized("Invalid email or password.")
        };
    }

    [HttpPost("CreateUser")]
    public async Task<IActionResult> CreateUser([FromBody] WebUserDto userDTO)
    {
        if (userDTO == null)
            return BadRequest("Request body is missing.");

        if (string.IsNullOrWhiteSpace(userDTO.Email))
            return BadRequest("Email is required.");

        if (string.IsNullOrWhiteSpace(userDTO.FullName))
            return BadRequest("Full name is required.");

        if (string.IsNullOrWhiteSpace(userDTO.Username))
            return BadRequest("Username is required.");

        var result = await _accountService.CreateAccountAsync(userDTO.FullName, userDTO.Username, userDTO.Email, userDTO.Password);
        return result.Status switch
        {
            CreateAccountStatus.Created => Ok(new
            {
                message = "User created successfully",
                username = result.UserName,
                email = result.Email
            }),
            CreateAccountStatus.EmailInUse => BadRequest("Email already in use."),
            _ => BadRequest(result.Errors)
        };
    }

    [HttpGet("CheckDuplicateEmail")]
    public Task<bool> GetUserEmail(string email)
    {
        return _accountService.EmailExistsAsync(email);
    }

    [HttpGet("FindByUser")]
    public Task<bool> GetUser(string username)
    {
        return _accountService.UserNameExistsAsync(username);
    }

    [HttpGet("ReturnAllUsers")]
    public async Task<IActionResult> GetAllUsers()
    {
        return Ok(await _accountService.GetAllUsersAsync());
    }

    [Authorize]
    [HttpGet("GetTheCurrentUser")]
    public async Task<IActionResult> GetTheCurrentUser()
    {
        _logger.LogCritical(HttpContext.User.Identity?.Name);
        var user = await _accountService.GetCurrentUserAsync(HttpContext.User);
        return user is not null ? Ok(user) : Unauthorized();
    }

    [Authorize]
    [HttpGet("IsUserLoggedIn")]
    public async Task<IActionResult> CheckIsUserLoggedIn()
    {
        _logger.LogCritical("--------------------------------------------------------------");
        _logger.LogCritical("from the httpcontext user identity");
        _logger.LogCritical(HttpContext.User.Identity?.Name);

        var user = await _accountService.GetCurrentUserAsync(HttpContext.User);
        if (user is null)
        {
            return Unauthorized();
        }

        _logger.LogCritical(user.Email);
        return Ok(true);
    }

    [Authorize]
    [HttpGet("user")]
    public IActionResult GetUser()
    {
        _logger.LogInformation("User is authenticated");
        return Ok("youur logged in bro ");
    }
}
