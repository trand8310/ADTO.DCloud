using ADTOSharp.Domain.Uow;
using ADTOSharp.EntityFrameworkCore;
using ADTOSharp.Modules;
using ADTOSharp.MultiTenancy;
using ADTOSharp.Reflection.Extensions;
using Castle.MicroKernel.Registration;

namespace ADTOSharp.Zero.EntityFrameworkCore;

/// <summary>
/// Entity framework integration module for ASP.NET Boilerplate Zero.
/// </summary>
[DependsOn(typeof(ADTOSharpZeroCoreModule), typeof(ADTOSharpEntityFrameworkCoreModule))]
public class ADTOSharpZeroCoreEntityFrameworkCoreModule : ADTOSharpModule
{
    public override void PreInitialize()
    {
        Configuration.ReplaceService(typeof(IConnectionStringResolver), () =>
        {
            IocManager.IocContainer.Register(
                Component.For<IConnectionStringResolver, IDbPerTenantConnectionStringResolver>()
                    .ImplementedBy<DbPerTenantConnectionStringResolver>()
                    .LifestyleTransient()
                );
        });
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(ADTOSharpZeroCoreEntityFrameworkCoreModule).GetAssembly());
    }
}