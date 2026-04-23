using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ADTOSharp.Authorization.Roles;
using ADTOSharp.Authorization.Users;
using ADTOSharp.Dependency;
using ADTOSharp.Domain.Uow;
using ADTOSharp.Runtime.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace ADTOSharp.Authorization;

public class ADTOSharpUserClaimsPrincipalFactory<TUser, TRole> : UserClaimsPrincipalFactory<TUser, TRole>, ITransientDependency
    where TRole : ADTOSharpRole<TUser>, new()
    where TUser : ADTOSharpUser<TUser>
{
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public ADTOSharpUserClaimsPrincipalFactory(
        ADTOSharpUserManager<TRole, TUser> userManager,
        ADTOSharpRoleManager<TRole, TUser> roleManager,
        IOptions<IdentityOptions> optionsAccessor,
        IUnitOfWorkManager unitOfWorkManager) : base(userManager, roleManager, optionsAccessor)
    {
        _unitOfWorkManager = unitOfWorkManager;
    }

    public override async Task<ClaimsPrincipal> CreateAsync(TUser user)
    {
        return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
        {
            var principal = await base.CreateAsync(user);

            if (user.TenantId.HasValue)
            {
                principal.Identities.First().AddClaim(new Claim(ADTOSharpClaimTypes.TenantId, user.TenantId.ToString()));
            }

            return principal;
        });
    }
}