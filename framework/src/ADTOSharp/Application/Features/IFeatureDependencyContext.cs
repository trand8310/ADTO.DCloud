using ADTOSharp.Dependency;
using System;

namespace ADTOSharp.Application.Features
{
    /// <summary>
    /// Used in the <see cref="IFeatureDependency.IsSatisfiedAsync"/> method.
    /// </summary>
    public interface IFeatureDependencyContext
    {
        /// <summary>
        /// Tenant id which requires the feature.
        /// Null for current tenant.
        /// </summary>
        Guid? TenantId { get; }

        /// <summary>
        /// Gets the <see cref="IIocResolver"/>.
        /// </summary>
        /// <value>
        /// The ioc resolver.
        /// </value>
        IIocResolver IocResolver { get; }

        /// <summary>
        /// Gets the <see cref="IFeatureChecker"/>.
        /// </summary>
        /// <value>
        /// The feature checker.
        /// </value>
        IFeatureChecker FeatureChecker { get; }
    }
}