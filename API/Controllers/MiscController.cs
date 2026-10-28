using Microsoft.AspNetCore.Mvc;
using Resturant.Domain.Entity;
using Resturant.Infrastructure.Context;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MiscController : Controller
{
    
    private readonly RestaurantContext _context;
    
    public MiscController(RestaurantContext context)
    {
        _context = context;
    }
    
    [HttpPost("BookingInformation")]
    public async Task<IActionResult> BookingInformation(Reservation bookingInformation)
    {
        await _context.Reservations.AddAsync(bookingInformation);
        await _context.SaveChangesAsync();
        return Ok(bookingInformation);
    }
    
        
    [HttpPost("CustomerInquiry")]
    public async Task<IActionResult> PostContactAsync(CustomerInquiry contact)
    {
        await _context.CustomerInquiries.AddAsync(contact);
        await _context.SaveChangesAsync();
        return Ok(contact);
    }
    
    
    [HttpGet("CreateGuideForFrontend")]
    public IActionResult MakeGuid()
    {
        var guid = Guid.NewGuid();
        return Ok(guid);
    }

    [HttpGet("Test")]
    public string Test()
    {
        return "Test";
    }

    
}