using API.Common;
using Resturant.Domain.ContactInfo;
using Resturant.Domain.Repositories;

namespace API.Features.Contact.CreateInquiry;

/// <summary>Saves a contact-form inquiry.</summary>
public class CreateInquiryHandler : IHandler
{
    private readonly IContactRepository _contactRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateInquiryHandler(IContactRepository contactRepository, IUnitOfWork unitOfWork)
    {
        _contactRepository = contactRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>Saves an inquiry. Any client-supplied id is ignored; the returned DTO carries the generated id.</summary>
    public async Task<CustomerInquiryDto> HandleAsync(CustomerInquiryDto inquiry, CancellationToken cancellationToken = default)
    {
        var entity = new CustomerInquiry(inquiry.Name, inquiry.Email, inquiry.Subject, inquiry.Message);
        await _contactRepository.AddInquiryAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ToDto(entity);
    }

    private static CustomerInquiryDto ToDto(CustomerInquiry entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Email = entity.Email,
        Subject = entity.Subject,
        Message = entity.Message
    };
}
