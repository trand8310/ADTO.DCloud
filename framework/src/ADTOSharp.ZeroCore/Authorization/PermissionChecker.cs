using System;
using System.Threading.Tasks;
using ADTOSharp.Authorization.Roles;
using ADTOSharp.Authorization.Users;
using ADTOSharp.Dependency;
using ADTOSharp.Domain.Uow;
using ADTOSharp.Runtime.Session;
using Castle.Core.Logging;

namespace ADTOSharp.Authorization;

/// <summary>
/// Application should inherit this class to implement <see cref="IPermissionChecker"/>.
/// </summary>
/// <typeparam name="TRole"></typeparam>
/// <typeparam name="TUser"></typeparam>
public class PermissionChecker<TRole, TUser> : IPermissionChecker, ITransientDependency, IIocManagerAccessor
    where TRole : ADTOSharpRole<TUser>, new()
    where TUser : ADTOSharpUser<TUser>
{
    private readonly ADTOSharpUserManager<TRole, TUser> _userManager;

    public IIocManager IocManager { get; set; }

    public ILogger Logger { get; set; }

    public IADTOSharpSession ADTOSharpSession { get; set; }

    public ICurrentUnitOfWorkProvider CurrentUnitOfWorkProvider { get; set; }

    public IUnitOfWorkManager UnitOfWorkManager { get; set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    public PermissionChecker(ADTOSharpUserManager<TRole, TUser> userManager)
    {
        _userManager = userManager;

        Logger = NullLogger.Instance;
        ADTOSharpSession = NullADTOSharpSession.Instance;
    }

    public virtual async Task<bool> IsGrantedAsync(string permissionName)
    {
        return ADTOSharpSession.UserId.HasValue && await IsGrantedAsync(ADTOSharpSession.UserId.Value, permissionName);
    }

    public virtual bool IsGranted(string permissionName)
    {
        return ADTOSharpSession.UserId.HasValue && IsGranted(ADTOSharpSession.UserId.Value, permissionName);
    }

    public virtual async Task<bool> IsGrantedAsync(Guid userId, string permissionName)
    {
        return await _userManager.IsGrantedAsync(userId, permissionName);
    }

    public virtual bool IsGranted(Guid userId, string permissionName)
    {
        return _userManager.IsGranted(userId, permissionName);
    }

    public virtual async Task<bool> IsGrantedAsync(UserIdentifier user, string permissionName)
    {
        return await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
        {
            if (CurrentUnitOfWorkProvider?.Current == null)
            {
                return await IsGrantedAsync(user.UserId, permissionName);
            }

            using (CurrentUnitOfWorkProvider.Current.SetTenantId(user.TenantId))
            {
                return await IsGrantedAsync(user.UserId, permissionName);
            }
        });
    }

    public virtual bool IsGranted(UserIdentifier user, string permissionName)
    {
        return UnitOfWorkManager.WithUnitOfWork(() =>
        {
            if (CurrentUnitOfWorkProvider?.Current == null)
            {
                return IsGranted(user.UserId, permissionName);
            }

            using (CurrentUnitOfWorkProvider.Current.SetTenantId(user.TenantId))
            {
                return IsGranted(user.UserId, permissionName);
            }
        });
    }
}