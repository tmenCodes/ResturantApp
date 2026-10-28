using Resturant.Business.Dtos;
using Resturant.Data.Identity;

namespace Resturant.Business.Mappings;

internal static class AccountMappings
{
    public static AccountUserDto ToDto(this WebUser user) =>
        new(user.Id, user.UserName, user.Email, user.FullName);
}
