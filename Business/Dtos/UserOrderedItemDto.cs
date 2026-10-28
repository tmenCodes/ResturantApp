namespace Resturant.Business.Dtos;

/// <summary>One item from a past order, formatted for display.</summary>
public record UserOrderedItemDto(string Date, string Identity, string Name, string Price);
