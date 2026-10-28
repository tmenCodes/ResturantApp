using Resturant.Business.Dtos;
using Resturant.Business.Services;
using Resturant.Data.Repositories;

namespace Business.Tests;

public class ContactServiceTests
{
    [Fact]
    public async Task CreateReservationAsync_SavesReservationAndReturnsGeneratedId()
    {
        await using var context = TestDatabase.Create();
        var service = new ContactService(new ContactRepository(context));

        var result = await service.CreateReservationAsync(new ReservationDto
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
    public async Task CreateReservationAsync_IgnoresClientSuppliedId()
    {
        await using var context = TestDatabase.Create();
        var service = new ContactService(new ContactRepository(context));

        var first = await service.CreateReservationAsync(new ReservationDto { Id = 999, Name = "First" });
        var second = await service.CreateReservationAsync(new ReservationDto { Id = 999, Name = "Second" });

        Assert.NotEqual(999, first.Id);
        Assert.NotEqual(first.Id, second.Id);
        Assert.Equal(2, context.Reservations.Count());
    }

    [Fact]
    public async Task CreateInquiryAsync_SavesInquiryAndReturnsGeneratedId()
    {
        await using var context = TestDatabase.Create();
        var service = new ContactService(new ContactRepository(context));

        var result = await service.CreateInquiryAsync(new CustomerInquiryDto
        {
            Name = "Grace",
            Email = "grace@example.com",
            Subject = "Catering",
            Message = "Do you cater events?"
        });

        var saved = Assert.Single(context.CustomerInquiries);
        Assert.True(result.Id > 0);
        Assert.Equal(saved.Id, result.Id);
        Assert.Equal("Catering", saved.Subject);
        Assert.Equal("Grace", result.Name);
    }
}
