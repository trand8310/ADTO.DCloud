using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using ADTOSharp.Application.Features;
using ADTOSharp.Collections.Extensions;
using ADTOSharp.Configuration.Startup;
using ADTOSharp.Dependency;
using ADTOSharp.Domain.Uow;
using ADTOSharp.MultiTenancy;
using ADTOSharp.Runtime.Session;

namespace ADTOSharp.Authorization
{
    /// <summary>
    /// Permission manager.
    /// </summary>
    public class PermissionManager : PermissionDefinitionContextBase, IPermissionManager, ISingletonDependency
    {
        public IADTOSharpSession ADTOSharpSession { get; set; }

        private readonly IIocManager _iocManager;
        private readonly IAuthorizationConfiguration _authorizationConfiguration;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IMultiTenancyConfig _multiTenancy;

        /// <summary>
        /// Constructor.
        /// </summary>
        public PermissionManager(
            IIocManager iocManager,
            IAuthorizationConfiguration authorizationConfiguration,
            IUnitOfWorkManager unitOfWorkManager,
            IMultiTenancyConfig multiTenancy)
        {
            _iocManager = iocManager;
            _authorizationConfiguration = authorizationConfiguration;
            _unitOfWorkManager = unitOfWorkManager;
            _multiTenancy = multiTenancy;

            ADTOSharpSession = NullADTOSharpSession.Instance;
        }

        public virtual void Initialize()
        {
            foreach (var providerType in _authorizationConfiguration.Providers)
            {
                using (var provider = _iocManager.ResolveAsDisposable<AuthorizationProvider>(providerType))
                {
                    provider.Object.SetPermissions(this);
                }
            }

            Permissions.AddAllPermissions();
        }

        public virtual Permission GetPermission(string name)
        {
            var permission = Permissions.GetOrDefault(name);
            if (permission == null)
            {
                throw new ADTOSharpException("There is no permission with name: " + name);
            }

            return permission;
        }

        public virtual IReadOnlyList<Permission> GetAllPermissions(bool tenancyFilter = true)
        {
            using (var featureDependencyContext = _iocManager.ResolveAsDisposable<FeatureDependencyContext>())
            {
                var featureDependencyContextObject = featureDependencyContext.Object;
                featureDependencyContextObject.TenantId = GetCurrentTenantId();

                return Permissions.Values
                    .WhereIf(tenancyFilter, p => p.MultiTenancySides.HasFlag(GetCurrentMultiTenancySide()))
                    .Where(p =>
                        p.FeatureDependency == null ||
                        GetCurrentMultiTenancySide() == MultiTenancySides.Host ||
                        p.FeatureDependency.IsSatisfied(featureDependencyContextObject)
                    ).ToImmutableList();
            }
        }

        public virtual async Task<IReadOnlyList<Permission>> GetAllPermissionsAsync(bool tenancyFilter = true)
        {
            using (var featureDependencyContext = _iocManager.ResolveAsDisposable<FeatureDependencyContext>())
            {
                var featureDependencyContextObject = featureDependencyContext.Object;
                featureDependencyContextObject.TenantId = GetCurrentTenantId();

                var permissions = Permissions.Values
                    .WhereIf(tenancyFilter, p => p.MultiTenancySides.HasFlag(GetCurrentMultiTenancySide()))
                    .ToList();

                var result = await FilterSatisfiedPermissionsAsync(
                    featureDependencyContextObject,
                    permissions,
                    p => p.FeatureDependency == null || GetCurrentMultiTenancySide() == MultiTenancySides.Host
                );

                return result.ToImmutableList();
            }
        }

        public virtual IReadOnlyList<Permission> GetAllPermissions(MultiTenancySides multiTenancySides)
        {
            using (var featureDependencyContext = _iocManager.ResolveAsDisposable<FeatureDependencyContext>())
            {
                var featureDependencyContextObject = featureDependencyContext.Object;
                featureDependencyContextObject.TenantId = GetCurrentTenantId();

                return Permissions.Values
                    .Where(p => p.MultiTenancySides.HasFlag(multiTenancySides))
                    .Where(p =>
                        p.FeatureDependency == null ||
                        GetCurrentMultiTenancySide() == MultiTenancySides.Host ||
                        (p.MultiTenancySides.HasFlag(MultiTenancySides.Host) &&
                         multiTenancySides.HasFlag(MultiTenancySides.Host)) ||
                        p.FeatureDependency.IsSatisfied(featureDependencyContextObject)
                    ).ToImmutableList();
            }
        }

        public virtual async Task<IReadOnlyList<Permission>> GetAllPermissionsAsync(MultiTenancySides multiTenancySides)
        {
            using (var featureDependencyContext = _iocManager.ResolveAsDisposable<FeatureDependencyContext>())
            {
                var featureDependencyContextObject = featureDependencyContext.Object;
                featureDependencyContextObject.TenantId = GetCurrentTenantId();

                var permissions = Permissions.Values
                    .Where(p => p.MultiTenancySides.HasFlag(multiTenancySides))
                    .ToList();

                var result = await FilterSatisfiedPermissionsAsync(
                    featureDependencyContextObject,
                    permissions,
                    p =>
                        p.FeatureDependency == null ||
                        GetCurrentMultiTenancySide() == MultiTenancySides.Host ||
                        (p.MultiTenancySides.HasFlag(MultiTenancySides.Host) &&
                         multiTenancySides.HasFlag(MultiTenancySides.Host))
                );

                return result.ToImmutableList();
            }
        }

        private async Task<IList<Permission>> FilterSatisfiedPermissionsAsync(
            FeatureDependencyContext featureDependencyContextObject,
            IList<Permission> unfilteredPermissions,
            Func<Permission, bool> filter)
        {
            var filteredPermissions = new List<Permission>();

            foreach (var permission in unfilteredPermissions)
            {
                if (!filter.Invoke(permission) &&
                    !await permission.FeatureDependency.IsSatisfiedAsync(featureDependencyContextObject))
                {
                    continue;
                }
                
                filteredPermissions.Add(permission);
            }

            return filteredPermissions;
        }

        private MultiTenancySides GetCurrentMultiTenancySide()
        {
            if (_unitOfWorkManager.Current != null)
            {
                return _multiTenancy.IsEnabled && !_unitOfWorkManager.Current.GetTenantId().HasValue
                    ? MultiTenancySides.Host
                    : MultiTenancySides.Tenant;
            }

            return ADTOSharpSession.MultiTenancySide;
        }

        private Guid? GetCurrentTenantId()
        {
            if (_unitOfWorkManager.Current != null)
            {
                return _unitOfWorkManager.Current.GetTenantId();
            }

            return ADTOSharpSession.TenantId;
        }
    }
}