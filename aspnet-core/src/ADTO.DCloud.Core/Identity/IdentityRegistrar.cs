using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ADTO.DCloud.Authorization;
using ADTO.DCloud.Authorization.Roles;
using ADTO.DCloud.Authorization.Users;
using ADTO.DCloud.Editions;
using ADTO.DCloud.MultiTenancy;

namespace ADTO.DCloud.Identity
{
    public static class IdentityRegistrar
    {
        public static IdentityBuilder Register(IServiceCollection services)
        {
            services.AddLogging();

            return services.AddADTOSharpIdentity<Tenant, User, Role>()
                .AddADTOSharpTenantManager<TenantManager>()
                .AddADTOSharpUserManager<UserManager>()
                .AddADTOSharpRoleManager<RoleManager>()
                .AddADTOSharpEditionManager<EditionManager>()
                .AddADTOSharpUserStore<UserStore>()
                .AddADTOSharpRoleStore<RoleStore>()
                .AddADTOSharpLogInManager<LogInManager>()
                .AddADTOSharpSignInManager<SignInManager>()
                .AddADTOSharpSecurityStampValidator<SecurityStampValidator>()
                .AddADTOSharpUserClaimsPrincipalFactory<UserClaimsPrincipalFactory>()
                .AddPermissionChecker<PermissionChecker>()
                .AddDefaultTokenProviders();
        }
    }
}
