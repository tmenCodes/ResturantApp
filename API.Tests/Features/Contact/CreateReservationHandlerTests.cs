using API.Features.Contact.CreateReservation;
using Resturant.Infrastructure.Persistence;
using Resturant.Infrastructure.Persistence.Repositories;

namespace API.Tests.Features.Contact;

public class CreateReservationHandlerTests
{
    private static CreateReservationHandler CreateHandler(RestaurantContext context) =>
        new(new ContactRepository(context), new EfUnitOfWork(context));

    [Fact]
    public async Task HandleAsync_SavesReservationAndReturnsGeneratedId()
    {
        await using var context = TestDatabase.Create();

        var result = await CreateHandler(context).HandleAsync(new ReservationDto
        {
            Name = "Ada",
            Email = "ada@example.com",
            Phone = "555-0100",
            Date = "2026-11-01",
            Time = "19:00",
            People = 4,
            Message = "Window seat"
        });

        var saved = Assert.Single(context.Reservations);
        Assert.True(result.Id > 0);
        Assert.Equal(saved.Id, result.Id);
        Assert.Equal("Ada", saved.Name);
        Assert.Equal(4, saved.People);
        Assert.Equal("Window seat", result.Message);
    }

    [Fact]
    public async Task HandleAsync_IgnoresClientSuppliedId()
    {
        await using var context = TestDatabase.Create();
        var handler = CreateHandler(context);

        var first = await handler.HandleAsync(new ReservationDto { Id = 999, Name = "First" });
        var second = await handler.HandleAsync(new ReservationDto { Id = 999, Name = "Second" });

        Assert.NotEqual(999, first.Id);
        Assert.NotEqual(first.Id, second.Id);
        Assert.Equal(2, context.Reservations.Count());
    }
}
