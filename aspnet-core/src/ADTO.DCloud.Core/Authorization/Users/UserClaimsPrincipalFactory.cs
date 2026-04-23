using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using ADTOSharp.Authorization;
using ADTO.DCloud.Authorization.Roles;
using ADTOSharp.Domain.Uow;

namespace ADTO.DCloud.Authorization.Users;

public class UserClaimsPrincipalFactory : ADTOSharpUserClaimsPrincipalFactory<User, Role>
{
    public UserClaimsPrincipalFactory(
        UserManager userManager,
        RoleManager roleManager,
        IOptions<IdentityOptions> optionsAccessor,
        IUnitOfWorkManager unitOfWorkManager)
        : base(
              userManager,
              roleManager,
              optionsAccessor,
              unitOfWorkManager)
    {
    }
}
