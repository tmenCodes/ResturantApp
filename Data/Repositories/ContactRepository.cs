using Resturant.Data.Context;
using Resturant.Data.Entities;

namespace Resturant.Data.Repositories;

/// <summary>EF Core implementation of <see cref="IContactRepository"/>.</summary>
public class ContactRepository : IContactRepository
{
    private readonly RestaurantContext _context;

    public ContactRepository(RestaurantContext context)
    {
        _context = context;
    }

    public async Task AddReservationAsync(Reservation reservation, CancellationToken cancellationToken = default)
    {
        await _context.Reservations.AddAsync(reservation, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddInquiryAsync(CustomerInquiry inquiry, CancellationToken cancellationToken = default)
    {
        await _context.CustomerInquiries.AddAsync(inquiry, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
