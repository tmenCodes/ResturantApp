using Microsoft.AspNetCore.Mvc;
using Resturant.Business.Dtos;
using Resturant.Business.Services;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MiscController : Controller
{

    private readonly IContactService _contactService;

    public MiscController(IContactService contactService)
    {
        _contactService = contactService;
    }

    [HttpPost("BookingInformation")]
    public async Task<IActionResult> BookingInformation(ReservationDto bookingInformation, CancellationToken cancellationToken)
    {
        var reservation = await _contactService.CreateReservationAsync(bookingInformation, cancellationToken);
        return Ok(reservation);
    }


    [HttpPost("CustomerInquiry")]
    public async Task<IActionResult> PostContactAsync(CustomerInquiryDto contact, CancellationToken cancellationToken)
    {
        var inquiry = await _contactService.CreateInquiryAsync(contact, cancellationToken);
        return Ok(inquiry);
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
