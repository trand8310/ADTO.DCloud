using ADTOSharp.Application.Features;
using ADTOSharp.Domain.Entities;
using System;

namespace ADTOSharp.MultiTenancy
{
    /// <summary>
    /// Feature setting for a Tenant (<see cref="ADTOSharpTenant{TUser}"/>).
    /// </summary>
    public class TenantFeatureSetting : FeatureSetting
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TenantFeatureSetting"/> class.
        /// </summary>
        public TenantFeatureSetting()
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantFeatureSetting"/> class.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="name">Feature name.</param>
        /// <param name="value">Feature value.</param>
        public TenantFeatureSetting(Guid tenantId, string name, string value)
            :base(name, value)
        {
            TenantId = tenantId;
        }
    }
}