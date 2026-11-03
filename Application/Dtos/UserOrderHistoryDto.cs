namespace Resturant.Application.Dtos;

/// <summary>The total and short date of one past order.</summary>
public record UserOrderHistoryDto(decimal TotalPrice, string Date);
