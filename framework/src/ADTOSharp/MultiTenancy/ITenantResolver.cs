using System;
using System.Threading.Tasks;

namespace ADTOSharp.MultiTenancy
{
    public interface ITenantResolver
    {
        Guid? ResolveTenantId();
        
        Task<Guid?> ResolveTenantIdAsync();
    }
}
