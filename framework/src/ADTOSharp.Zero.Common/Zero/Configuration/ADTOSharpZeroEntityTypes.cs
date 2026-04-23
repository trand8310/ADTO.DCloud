using System;
using System.Reflection;
using ADTOSharp.Authorization.Roles;
using ADTOSharp.Authorization.Users;
using ADTOSharp.MultiTenancy;

namespace ADTOSharp.Zero.Configuration
{
    public class ADTOSharpZeroEntityTypes : IADTOSharpZeroEntityTypes
    {
        public Type User
        {
            get { return _user; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (!typeof (ADTOSharpUserBase).IsAssignableFrom(value))
                {
                    throw new ADTOSharpException(value.AssemblyQualifiedName + " should be derived from " + typeof(ADTOSharpUserBase).AssemblyQualifiedName);
                }

                _user = value;
            }
        }
        private Type _user;

        public Type Role
        {
            get { return _role; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (!typeof(ADTOSharpRoleBase).IsAssignableFrom(value))
                {
                    throw new ADTOSharpException(value.AssemblyQualifiedName + " should be derived from " + typeof(ADTOSharpRoleBase).AssemblyQualifiedName);
                }

                _role = value;
            }
        }
        private Type _role;

        public Type Tenant
        {
            get { return _tenant; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (!typeof(ADTOSharpTenantBase).IsAssignableFrom(value))
                {
                    throw new ADTOSharpException(value.AssemblyQualifiedName + " should be derived from " + typeof(ADTOSharpTenantBase).AssemblyQualifiedName);
                }

                _tenant = value;
            }
        }
        private Type _tenant;
    }
}