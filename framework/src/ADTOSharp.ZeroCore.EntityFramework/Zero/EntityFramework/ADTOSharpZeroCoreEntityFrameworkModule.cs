using ADTOSharp.Domain.Uow;
using ADTOSharp.EntityFramework;
using ADTOSharp.Modules;
using ADTOSharp.MultiTenancy;
using ADTOSharp.Reflection.Extensions;
using Castle.MicroKernel.Registration;

namespace ADTOSharp.Zero.EntityFramework;

/// <summary>
/// Entity framework integration module for ASP.NET Boilerplate Zero.
/// </summary>
[DependsOn(typeof(ADTOSharpZeroCoreModule), typeof(ADTOSharpEntityFrameworkModule))]
public class ADTOSharpZeroCoreEntityFrameworkModule : ADTOSharpModule
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
        IocManager.RegisterAssemblyByConvention(typeof(ADTOSharpZeroCoreEntityFrameworkModule).GetAssembly());
    }
}