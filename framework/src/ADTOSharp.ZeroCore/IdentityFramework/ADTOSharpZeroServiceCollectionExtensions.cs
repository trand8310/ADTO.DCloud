using System;
using ADTOSharp.Application.Editions;
using ADTOSharp.Application.Features;
using ADTOSharp.Authorization;
using ADTOSharp.Authorization.Roles;
using ADTOSharp.Authorization.Users;
using ADTOSharp.MultiTenancy;
using ADTOSharp.Zero.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace - This is done to add extension methods to Microsoft.Extensions.DependencyInjection namespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ADTOSharpZeroServiceCollectionExtensions
{
    public static ADTOSharpIdentityBuilder AddADTOSharpIdentity<TTenant, TUser, TRole>(this IServiceCollection services)
        where TTenant : ADTOSharpTenant<TUser>
        where TRole : ADTOSharpRole<TUser>, new()
        where TUser : ADTOSharpUser<TUser>
    {
        return services.AddADTOSharpIdentity<TTenant, TUser, TRole>(setupAction: null);
    }

    public static ADTOSharpIdentityBuilder AddADTOSharpIdentity<TTenant, TUser, TRole>(this IServiceCollection services, Action<IdentityOptions> setupAction)
        where TTenant : ADTOSharpTenant<TUser>
        where TRole : ADTOSharpRole<TUser>, new()
        where TUser : ADTOSharpUser<TUser>
    {
        services.AddSingleton<IADTOSharpZeroEntityTypes>(new ADTOSharpZeroEntityTypes
        {
            Tenant = typeof(TTenant),
            Role = typeof(TRole),
            User = typeof(TUser)
        });

        //ADTOSharpTenantManager
        services.TryAddScoped<ADTOSharpTenantManager<TTenant, TUser>>();

        //ADTOSharpEditionManager
        services.TryAddScoped<ADTOSharpEditionManager>();

        //ADTOSharpRoleManager
        services.TryAddScoped<ADTOSharpRoleManager<TRole, TUser>>();
        services.TryAddScoped(typeof(RoleManager<TRole>), provider => provider.GetService(typeof(ADTOSharpRoleManager<TRole, TUser>)));

        //ADTOSharpUserManager
        services.TryAddScoped<ADTOSharpUserManager<TRole, TUser>>();
        services.TryAddScoped(typeof(UserManager<TUser>), provider => provider.GetService(typeof(ADTOSharpUserManager<TRole, TUser>)));

        //SignInManager
        services.TryAddScoped<ADTOSharpSignInManager<TTenant, TRole, TUser>>();
        services.TryAddScoped(typeof(SignInManager<TUser>), provider => provider.GetService(typeof(ADTOSharpSignInManager<TTenant, TRole, TUser>)));

        //ADTOSharpLogInManager
        services.TryAddScoped<ADTOSharpLogInManager<TTenant, TRole, TUser>>();

        //ADTOSharpUserClaimsPrincipalFactory
        services.TryAddScoped<ADTOSharpUserClaimsPrincipalFactory<TUser, TRole>>();
        services.TryAddScoped(typeof(UserClaimsPrincipalFactory<TUser, TRole>), provider => provider.GetService(typeof(ADTOSharpUserClaimsPrincipalFactory<TUser, TRole>)));
        services.TryAddScoped(typeof(IUserClaimsPrincipalFactory<TUser>), provider => provider.GetService(typeof(ADTOSharpUserClaimsPrincipalFactory<TUser, TRole>)));

        //ADTOSharpSecurityStampValidator
        services.TryAddScoped<ADTOSharpSecurityStampValidator<TTenant, TRole, TUser>>();
        services.TryAddScoped(typeof(SecurityStampValidator<TUser>), provider => provider.GetService(typeof(ADTOSharpSecurityStampValidator<TTenant, TRole, TUser>)));
        services.TryAddScoped(typeof(ISecurityStampValidator), provider => provider.GetService(typeof(ADTOSharpSecurityStampValidator<TTenant, TRole, TUser>)));

        //PermissionChecker
        services.TryAddScoped<PermissionChecker<TRole, TUser>>();
        services.TryAddScoped(typeof(IPermissionChecker), provider => provider.GetService(typeof(PermissionChecker<TRole, TUser>)));

        //ADTOSharpUserStore
        services.TryAddScoped<ADTOSharpUserStore<TRole, TUser>>();
        services.TryAddScoped(typeof(IUserStore<TUser>), provider => provider.GetService(typeof(ADTOSharpUserStore<TRole, TUser>)));

        //ADTOSharpRoleStore
        services.TryAddScoped<ADTOSharpRoleStore<TRole, TUser>>();
        services.TryAddScoped(typeof(IRoleStore<TRole>), provider => provider.GetService(typeof(ADTOSharpRoleStore<TRole, TUser>)));

        //ADTOSharpFeatureValueStore
        services.TryAddScoped<ADTOSharpFeatureValueStore<TTenant, TUser>>();
        services.TryAddScoped(typeof(IFeatureValueStore), provider => provider.GetService(typeof(ADTOSharpFeatureValueStore<TTenant, TUser>)));

        return new ADTOSharpIdentityBuilder(services.AddIdentity<TUser, TRole>(setupAction), typeof(TTenant));
    }
}