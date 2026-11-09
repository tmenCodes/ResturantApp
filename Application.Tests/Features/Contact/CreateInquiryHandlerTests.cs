using API.Features.Contact.CreateInquiry;
using Resturant.Infrastructure.Persistence;
using Resturant.Infrastructure.Persistence.Repositories;

namespace Application.Tests.Features.Contact;

public class CreateInquiryHandlerTests
{
    [Fact]
    public async Task HandleAsync_SavesInquiryAndReturnsGeneratedId()
    {
        await using var context = TestDatabase.Create();
        var handler = new CreateInquiryHandler(new ContactRepository(context), new EfUnitOfWork(context));

        var result = await handler.HandleAsync(new CustomerInquiryDto
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
