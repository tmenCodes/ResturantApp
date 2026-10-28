using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Resturant.Domain.Entity;
using Resturant.Infrastructure.Identity;
using API.Contracts;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RegisterUser : Controller
{
    private readonly UserManager<WebUser> _userManager;
    private readonly SignInManager<WebUser> _signInManager;
    private readonly ILogger<RegisterUser> _logger;
    private readonly IConfiguration _configuration;


    public RegisterUser(UserManager<WebUser> userManager, SignInManager<WebUser> signInManager,
        ILogger<RegisterUser> logger, IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
        _configuration = configuration;
    }



    // [HttpGet("CheckUserLogin")]
    // public bool IsUserCurrentlySignedIn(IdentityUser user)
    // {
    //     // This method can be used to check if a specific user is currently signed in
    //     // (e.g., if you have their IdentityUser object)
    //   //  return _signInManager.IsSignedIn(user);
    // }

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

        var user = await _userManager.FindByEmailAsync(login.Email);
        if (user is null)
            return Unauthorized("Invalid email or password.");

        var result = await _signInManager.CheckPasswordSignInAsync(
            user, login.Password, lockoutOnFailure: true);

        if (result.Succeeded)
        {
            _logger.LogInformation("User {UserName} logged in.", user.UserName);
            var token = GenerateJwtToken(user);
            return Ok(new { token, message = "Login successful" });
        }

        if (result.IsLockedOut)
            return StatusCode(429, "Account is locked. Try again later.");

        return Unauthorized("Invalid email or password.");
    }

    private string GenerateJwtToken(WebUser user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var expiresInMinutes = jwtSettings.GetValue<double?>("ExpiresInMinutes") ?? 60;

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    


    // [HttpPost("CreateUser")]
    // public async Task<IActionResult> CreateUser([FromBody] WebUserDto userDTO)
    // {
    //     string username = userDTO.Email?.Substring(0, userDTO.Email!.IndexOf('@'));
    //      _logger.LogInformation("--------------------------------------------------------------------");
    //     _logger.LogCritical("THIS IS THE USERNAME!!!!!!!" + username);
    //   //  string username = userDTO.Email!; 
    //     //convert here 
    //     WebUser user = new()
    //     {
    //         
    //         FullName = userDTO.FullName,
    //         //username is based of the beginning of email
    //         UserName = username, 
    //         Email = userDTO.Email,
    //
    //     };
    //     //do not allow duplicate emails 
    //     var email =  await _userManager.FindByEmailAsync(userDTO.Email);
    //     if (email == null)
    //     {
    //              //create new user-must fufill password requirements
    //             var result = await _userManager.CreateAsync(user, userDTO.Password);
    //             if (result.Succeeded)
    //             {
    //                 await _signInManager.SignInAsync(user, true);
    //                 return Ok(userDTO); 
    //             }
    //
    //             if (result.Errors.Any())
    //             {
    //                 _logger.LogWarning("User {UserName} could not be created.", user.UserName);
    //                 return BadRequest(new { message = "There was an error creating the account" });
    //             
    //             }
    //             // foreach (var error in result.Errors)
    //             // {
    //             //     _logger.LogInformation(error.Description);
    //             // }
    //             return Ok(result); 
    //     }
    //
    //     return BadRequest("The email is already in use");
    // }
    
    
    
    
    
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

        var email = userDTO.Email.Trim().ToLowerInvariant();
        var baseUsername = userDTO.Username.Trim().ToLowerInvariant();

        var existingEmailUser = await _userManager.FindByEmailAsync(email);
        if (existingEmailUser != null)
            return BadRequest("Email already in use.");

        var username = baseUsername;
        int suffix = 1;

        while (await _userManager.FindByNameAsync(username) != null)
        {
            username = $"{baseUsername}{suffix}";
            suffix++;
        }

        var user = new WebUser
        {
            FullName = userDTO.FullName.Trim(),
            UserName = username,
            Email = email
        };

        // 6. Create user in Identity
        var result = await _userManager.CreateAsync(user, userDTO.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Ok(new
        {
            message = "User created successfully",
            username,
            email
        });
    }

    
    [HttpGet("CheckDuplicateEmail")]
    public async Task<bool> GetUserEmail(string email)
    {
        var results =  await _userManager.FindByEmailAsync(email);
     
        if (results == null)
        {
            return false; 
        }
        return true; 
    
    }

    [HttpGet("FindByUser")]
    public async Task<bool> GetUser(string username)
    {
        try
        {
            var user = await _userManager.FindByNameAsync(username);
            string users = user.UserName;
            return true; 
        }
        catch
        {
            return false; 
        }
    }
    
    

    [HttpGet("ReturnAllUsers")]
    public async Task<IActionResult> GetAllUsers()
    {  
        await _userManager.GetUserAsync(HttpContext.User);
        var user =  await _userManager.Users.ToListAsync();
        return Ok(user); 
    }

    
    [Authorize]
    [HttpGet("GetTheCurrentUser")]
    public async Task<IActionResult> GetTheCurrentUser()
    {
           _logger.LogCritical(HttpContext.User.Identity.Name);
        //    _logger.LogDebug( User.Identity.IsAuthenticated.ToString()); 
        // //   _logger.LogInformation(HttpContext.User.ToString());
        // //   _logger.LogCritical(HttpContext.User.ToString());
        //    _logger.LogCritical(HttpContext.User.Identity.IsAuthenticated.ToString());
      var user = await _userManager.GetUserAsync(HttpContext.User);
  
      if (user != null)
      {
          return Ok(user);
      }
      return Unauthorized();
      //return Ok(false); 
    }

    [Authorize]
    [HttpGet("IsUserLoggedIn")]
    public async Task<IActionResult> CheckIsUserLoggedIn()
    {
        _logger.LogCritical("--------------------------------------------------------------");
        _logger.LogCritical("from the httpcontext user identity");
        _logger.LogCritical(HttpContext.User.Identity.Name);


//possibly wrap into try catch
        var user = await _userManager.GetUserAsync(HttpContext.User);

        if (user != null)
        {
            _logger.LogCritical(user.Email);
            return Ok(true);
        }
        return Unauthorized();
        //return Ok(false);
    }

    
    
    
    [Authorize]
    [HttpGet("user")]
    public IActionResult GetUser()
    {
        _logger.LogInformation("User is authenticated");
        return Ok("youur logged in bro ");
    }
    
    
}
