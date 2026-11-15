using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Resturant.Domain.Payments;

namespace Resturant.Infrastructure.Persistence.Configurations;

/// <summary>The column names a <see cref="CreditCardInformation"/> is stored in, shared by every entity that holds a card.</summary>
public static class CreditCardColumns
{
    /// <summary>
    /// Stores the card selected by <paramref name="cardNavigation"/> in the owner's own table, each field in its existing
    /// column, so holding a card needs no schema change. The card is required, so it loads even when every column is null.
    /// </summary>
    public static EntityTypeBuilder<TOwner> OwnsCardInOwnColumns<TOwner>(
        this EntityTypeBuilder<TOwner> owner, Expression<Func<TOwner, CreditCardInformation?>> cardNavigation)
        where TOwner : class
    {
        owner.OwnsOne(cardNavigation, card =>
        {
            card.Property(x => x.CardType).HasColumnName("Credit");
            card.Property(x => x.NameOnCard).HasColumnName("NameonCard");
            card.Property(x => x.Number).HasColumnName("CreditCardNumber");
            card.Property(x => x.Expiration).HasColumnName("Expiration");
            card.Property(x => x.Cvv).HasColumnName("CVV");
        });
        owner.Navigation(cardNavigation).IsRequired();
        return owner;
    }
}
