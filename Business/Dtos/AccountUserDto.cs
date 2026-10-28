namespace Resturant.Business.Dtos;

/// <summary>The public profile of a registered user. Never carries credentials or security stamps.</summary>
public record AccountUserDto(string Id, string? UserName, string? Email, string? FullName);
