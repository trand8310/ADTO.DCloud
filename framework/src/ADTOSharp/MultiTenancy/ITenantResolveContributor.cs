using System;

namespace ADTOSharp.MultiTenancy
{
    public interface ITenantResolveContributor
    {
        Guid? ResolveTenantId();
    }
}