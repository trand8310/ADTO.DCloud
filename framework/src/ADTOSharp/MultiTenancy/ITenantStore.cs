using JetBrains.Annotations;
using System;

namespace ADTOSharp.MultiTenancy
{
    public interface ITenantStore
    {
        [CanBeNull]
        TenantInfo Find(Guid tenantId);

        [CanBeNull]
        TenantInfo Find([NotNull] string tenancyName);
    }
}