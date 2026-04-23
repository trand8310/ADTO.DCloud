using ADTOSharp.MultiTenancy;

namespace ADTOSharp.Zero.EntityFrameworkCore;

public interface IMultiTenantSeed
{
    ADTOSharpTenantBase Tenant { get; set; }
}