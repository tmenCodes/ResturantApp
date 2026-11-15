using API.Common;
using Resturant.Domain.ContactInfo;
using Resturant.Infrastructure.Persistence;

namespace API.Features.Contact.CreateInquiry;

/// <summary>Saves a contact-form inquiry.</summary>
public class CreateInquiryHandler : IHandler
{
    private readonly RestaurantContext _context;

    public CreateInquiryHandler(RestaurantContext context)
    {
        _context = context;
    }

    /// <summary>Saves an inquiry. Any client-supplied id is ignored; the returned DTO carries the generated id.</summary>
    public async Task<CustomerInquiryDto> HandleAsync(CustomerInquiryDto inquiry, CancellationToken cancellationToken = default)
    {
        var entity = new CustomerInquiry(inquiry.Name, inquiry.Email, inquiry.Subject, inquiry.Message);
        await _context.CustomerInquiries.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return CustomerInquiryDto.From(entity);
    }
}
