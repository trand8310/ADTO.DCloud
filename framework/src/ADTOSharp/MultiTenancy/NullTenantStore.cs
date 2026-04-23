using System;

namespace ADTOSharp.MultiTenancy
{
    public class NullTenantStore : ITenantStore
    {
        public TenantInfo Find(Guid tenantId)
        {
            return null;
        }

        public TenantInfo Find(string tenancyName)
        {
            return null;
        }
    }
}