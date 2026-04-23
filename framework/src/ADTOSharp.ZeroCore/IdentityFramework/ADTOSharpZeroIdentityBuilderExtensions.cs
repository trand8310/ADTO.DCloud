using ADTOSharp.Application.Editions;
using ADTOSharp.Application.Features;
using ADTOSharp.Authorization;
using Microsoft.AspNetCore.Identity;
using ADTOSharp.Authorization.Users;
using ADTOSharp.Authorization.Roles;
using ADTOSharp.MultiTenancy;

// ReSharper disable once CheckNamespace - This is done to add extension methods to Microsoft.Extensions.DependencyInjection namespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ADTOSharpZeroIdentityBuilderExtensions
{
    public static ADTOSharpIdentityBuilder AddADTOSharpTenantManager<TTenantManager>(this ADTOSharpIdentityBuilder builder)
        where TTenantManager : class
    {
        var type = typeof(TTenantManager);
        var adtoManagerType = typeof(ADTOSharpTenantManager<,>).MakeGenericType(builder.TenantType, builder.UserType);
        builder.Services.AddScoped(type, provider => provider.GetRequiredService(adtoManagerType));
        builder.Services.AddScoped(adtoManagerType, type);
        return builder;
    }

    public static ADTOSharpIdentityBuilder AddADTOSharpEditionManager<TEditionManager>(this ADTOSharpIdentityBuilder builder)
        where TEditionManager : class
    {
        var type = typeof(TEditionManager);
        var adtoManagerType = typeof(ADTOSharpEditionManager);
        builder.Services.AddScoped(type, provider => provider.GetRequiredService(adtoManagerType));
        builder.Services.AddScoped(adtoManagerType, type);
        return builder;
    }

    public static ADTOSharpIdentityBuilder AddADTOSharpRoleManager<TRoleManager>(this ADTOSharpIdentityBuilder builder)
        where TRoleManager : class
    {
        var adtoManagerType = typeof(ADTOSharpRoleManager<,>).MakeGenericType(builder.RoleType, builder.UserType);
        var managerType = typeof(RoleManager<>).MakeGenericType(builder.RoleType);
        builder.Services.AddScoped(adtoManagerType, services => services.GetRequiredService(managerType));
        builder.AddRoleManager<TRoleManager>();
        return builder;
    }

    public static ADTOSharpIdentityBuilder AddADTOSharpUserManager<TUserManager>(this ADTOSharpIdentityBuilder builder)
        where TUserManager : class
    {
        var adtoManagerType = typeof(ADTOSharpUserManager<,>).MakeGenericType(builder.RoleType, builder.UserType);
        var managerType = typeof(UserManager<>).MakeGenericType(builder.UserType);
        builder.Services.AddScoped(adtoManagerType, services => services.GetRequiredService(managerType));
        builder.AddUserManager<TUserManager>();
        return builder;
    }

    public static ADTOSharpIdentityBuilder AddADTOSharpSignInManager<TSignInManager>(this ADTOSharpIdentityBuilder builder)
        where TSignInManager : class
    {
        var adtoManagerType = typeof(ADTOSharpSignInManager<,,>).MakeGenericType(builder.TenantType, builder.RoleType, builder.UserType);
        var managerType = typeof(SignInManager<>).MakeGenericType(builder.UserType);
        builder.Services.AddScoped(adtoManagerType, services => services.GetRequiredService(managerType));
        builder.AddSignInManager<TSignInManager>();
        return builder;
    }

    public static ADTOSharpIdentityBuilder AddADTOSharpLogInManager<TLogInManager>(this ADTOSharpIdentityBuilder builder)
        where TLogInManager : class
    {
        var type = typeof(TLogInManager);
        var adtoManagerType = typeof(ADTOSharpLogInManager<,,>).MakeGenericType(builder.TenantType, builder.RoleType, builder.UserType);
        builder.Services.AddScoped(type, provider => provider.GetService(adtoManagerType));
        builder.Services.AddScoped(adtoManagerType, type);
        return builder;
    }

    public static ADTOSharpIdentityBuilder AddADTOSharpUserClaimsPrincipalFactory<TUserClaimsPrincipalFactory>(this ADTOSharpIdentityBuilder builder)
        where TUserClaimsPrincipalFactory : class
    {
        var type = typeof(TUserClaimsPrincipalFactory);
        builder.Services.AddScoped(typeof(UserClaimsPrincipalFactory<,>).MakeGenericType(builder.UserType, builder.RoleType), services => services.GetRequiredService(type));
        builder.Services.AddScoped(typeof(ADTOSharpUserClaimsPrincipalFactory<,>).MakeGenericType(builder.UserType, builder.RoleType), services => services.GetRequiredService(type));
        builder.Services.AddScoped(typeof(IUserClaimsPrincipalFactory<>).MakeGenericType(builder.UserType), services => services.GetRequiredService(type));
        builder.Services.AddScoped(type);
        return builder;
    }

    public static ADTOSharpIdentityBuilder AddADTOSharpSecurityStampValidator<TSecurityStampValidator>(this ADTOSharpIdentityBuilder builder)
        where TSecurityStampValidator : class, ISecurityStampValidator
    {
        var type = typeof(TSecurityStampValidator);
        builder.Services.AddScoped(typeof(SecurityStampValidator<>).MakeGenericType(builder.UserType), services => services.GetRequiredService(type));
        builder.Services.AddScoped(typeof(ADTOSharpSecurityStampValidator<,,>).MakeGenericType(builder.TenantType, builder.RoleType, builder.UserType), services => services.GetRequiredService(type));
        builder.Services.AddScoped(typeof(ISecurityStampValidator), services => services.GetRequiredService(type));
        builder.Services.AddScoped(type);
        return builder;
    }

    public static ADTOSharpIdentityBuilder AddPermissionChecker<TPermissionChecker>(this ADTOSharpIdentityBuilder builder)
        where TPermissionChecker : class
    {
        var type = typeof(TPermissionChecker);
        var checkerType = typeof(PermissionChecker<,>).MakeGenericType(builder.RoleType, builder.UserType);
        builder.Services.AddScoped(type);
        builder.Services.AddScoped(checkerType, provider => provider.GetService(type));
        builder.Services.AddScoped(typeof(IPermissionChecker), provider => provider.GetService(type));
        return builder;
    }

    public static ADTOSharpIdentityBuilder AddADTOSharpUserStore<TUserStore>(this ADTOSharpIdentityBuilder builder)
        where TUserStore : class
    {
        var type = typeof(TUserStore);
        var adtoStoreType = typeof(ADTOSharpUserStore<,>).MakeGenericType(builder.RoleType, builder.UserType);
        var storeType = typeof(IUserStore<>).MakeGenericType(builder.UserType);
        builder.Services.AddScoped(type);
        builder.Services.AddScoped(adtoStoreType, services => services.GetRequiredService(type));
        builder.Services.AddScoped(storeType, services => services.GetRequiredService(type));
        return builder;
    }

    public static ADTOSharpIdentityBuilder AddADTOSharpRoleStore<TRoleStore>(this ADTOSharpIdentityBuilder builder)
        where TRoleStore : class
    {
        var type = typeof(TRoleStore);
        var adtoStoreType = typeof(ADTOSharpRoleStore<,>).MakeGenericType(builder.RoleType, builder.UserType);
        var storeType = typeof(IRoleStore<>).MakeGenericType(builder.RoleType);
        builder.Services.AddScoped(type);
        builder.Services.AddScoped(adtoStoreType, services => services.GetRequiredService(type));
        builder.Services.AddScoped(storeType, services => services.GetRequiredService(type));
        return builder;
    }

    public static ADTOSharpIdentityBuilder AddFeatureValueStore<TFeatureValueStore>(this ADTOSharpIdentityBuilder builder)
        where TFeatureValueStore : class
    {
        var type = typeof(TFeatureValueStore);
        var storeType = typeof(ADTOSharpFeatureValueStore<,>).MakeGenericType(builder.TenantType, builder.UserType);
        builder.Services.AddScoped(type);
        builder.Services.AddScoped(storeType, provider => provider.GetService(type));
        builder.Services.AddScoped(typeof(IFeatureValueStore), provider => provider.GetService(type));
        return builder;
    }
}