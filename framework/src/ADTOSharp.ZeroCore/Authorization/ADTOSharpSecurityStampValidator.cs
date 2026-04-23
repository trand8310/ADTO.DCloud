using System.Threading.Tasks;
using ADTOSharp.Authorization.Roles;
using ADTOSharp.Authorization.Users;
using ADTOSharp.Domain.Uow;
using ADTOSharp.MultiTenancy;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ADTOSharp.Authorization;

public class ADTOSharpSecurityStampValidator<TTenant, TRole, TUser> : SecurityStampValidator<TUser>
    where TTenant : ADTOSharpTenant<TUser>
    where TRole : ADTOSharpRole<TUser>, new()
    where TUser : ADTOSharpUser<TUser>
{
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public ADTOSharpSecurityStampValidator(
        IOptions<SecurityStampValidatorOptions> options,
        ADTOSharpSignInManager<TTenant, TRole, TUser> signInManager,
        ILoggerFactory loggerFactory,
        IUnitOfWorkManager unitOfWorkManager)
        : base(
            options,
            signInManager,
            loggerFactory)
    {
        _unitOfWorkManager = unitOfWorkManager;
    }

    public override async Task ValidateAsync(CookieValidatePrincipalContext context)
    {
        await _unitOfWorkManager.WithUnitOfWorkAsync(async () => { await base.ValidateAsync(context); });
    }
}