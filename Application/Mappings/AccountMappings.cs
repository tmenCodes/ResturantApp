using Resturant.Application.Dtos;
using Resturant.Infrastructure.Identity;

namespace Resturant.Application.Mappings;

internal static class AccountMappings
{
    public static AccountUserDto ToDto(this UserAccount account) =>
        new(account.Id, account.UserName, account.Email, account.FullName);
}
