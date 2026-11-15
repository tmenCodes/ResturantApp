using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Resturant.Domain.Customers;

namespace Resturant.Infrastructure.Persistence.Configurations;

/// <summary>Maps <see cref="SavedPaymentMethod"/>: its card is stored in the payment method's own columns.</summary>
public class SavedPaymentMethodConfiguration : IEntityTypeConfiguration<SavedPaymentMethod>
{
    public void Configure(EntityTypeBuilder<SavedPaymentMethod> builder)
    {
        builder.OwnsCardInOwnColumns(x => x.Card);
    }
}
