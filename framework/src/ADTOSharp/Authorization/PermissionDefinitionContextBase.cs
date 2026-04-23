using System.Collections.Generic;
using ADTOSharp.Application.Features;
using ADTOSharp.Collections.Extensions;
using ADTOSharp.Localization;
using ADTOSharp.MultiTenancy;

namespace ADTOSharp.Authorization
{
    public abstract class PermissionDefinitionContextBase : IPermissionDefinitionContext
    {
        protected readonly PermissionDictionary Permissions;

        protected PermissionDefinitionContextBase()
        {
            Permissions = new PermissionDictionary();
        }

        public Permission CreatePermission(
            string name,
            ILocalizableString displayName = null,
            ILocalizableString description = null,
            MultiTenancySides multiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant,
            IFeatureDependency featureDependency = null,
            Dictionary<string, object> properties = null)
        {
            if (Permissions.ContainsKey(name))
            {
                throw new ADTOSharpException("There is already a permission with name: " + name);
            }

            var permission = new Permission(name, displayName, description, multiTenancySides, featureDependency, properties);
            Permissions[permission.Name] = permission;
            return permission;
        }

        public virtual Permission GetPermissionOrNull(string name)
        {
            return Permissions.GetOrDefault(name);
        }

        public virtual void RemovePermission(string name)
        {
            Permissions.Remove(name);
        }
    }
}