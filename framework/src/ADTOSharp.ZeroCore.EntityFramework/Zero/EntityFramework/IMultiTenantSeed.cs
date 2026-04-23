using ADTOSharp.MultiTenancy;

namespace ADTOSharp.Zero.EntityFramework;

public interface IMultiTenantSeed
{
    ADTOSharpTenantBase Tenant { get; set; }
}