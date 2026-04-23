using ADTOSharp.Configuration.Startup;
using ADTOSharp.MultiTenancy;
using ADTOSharp.Runtime.Remoting;
using System;

namespace ADTOSharp.Runtime.Session
{
    /// <summary>
    /// Implements null object pattern for <see cref="IADTOSharpSession"/>.
    /// </summary>
    public class NullADTOSharpSession : ADTOSharpSessionBase
    {
        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static NullADTOSharpSession Instance { get; } = new NullADTOSharpSession();

        /// <inheritdoc/>
        public override Guid? UserId => null;

        /// <inheritdoc/>
        public override Guid? TenantId => null;

        public override MultiTenancySides MultiTenancySide => MultiTenancySides.Tenant;

        public override Guid? ImpersonatorUserId => null;

        public override Guid? ImpersonatorTenantId => null;

        private NullADTOSharpSession() 
            : base(
                  new MultiTenancyConfig(), 
                  new DataContextAmbientScopeProvider<SessionOverride>(new AsyncLocalAmbientDataContext())
            )
        {

        }
    }
}