using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Resturant.Domain.Ordering;

namespace Resturant.Infrastructure.Persistence.Configurations;

/// <summary>Maps <see cref="CheckoutInformation"/>: its card is stored in the checkout's own columns.</summary>
public class CheckoutInformationConfiguration : IEntityTypeConfiguration<CheckoutInformation>
{
    public void Configure(EntityTypeBuilder<CheckoutInformation> builder)
    {
        builder.OwnsCardInOwnColumns(x => x.Card);
    }
}
