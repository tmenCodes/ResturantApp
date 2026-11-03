using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Resturant.Domain.Ordering;

namespace Resturant.Infrastructure.Persistence.Configurations;

/// <summary>Maps <see cref="ShoppingCart"/>: order items reference their cart by its <see cref="ShoppingCart.Identity"/>.</summary>
public class ShoppingCartConfiguration : IEntityTypeConfiguration<ShoppingCart>
{
    public void Configure(EntityTypeBuilder<ShoppingCart> builder)
    {
        builder.HasMany(e => e.OrderItems)
            .WithOne(e => e.ShoppingCart)
            .HasForeignKey(e => e.ShoppingCartIdentity)
            .HasPrincipalKey(e => e.Identity);
    }
}
