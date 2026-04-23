using System;
using ADTOSharp.Dependency;
using Castle.MicroKernel.Registration;
using Microsoft.EntityFrameworkCore;

namespace ADTOSharp.EntityFrameworkCore.Configuration;

public class ADTOSharpEfCoreConfiguration : IADTOSharpEfCoreConfiguration
{
    private readonly IIocManager _iocManager;

    public ADTOSharpEfCoreConfiguration(IIocManager iocManager)
    {
        _iocManager = iocManager;
    }

    public bool UseADTOSharpQueryCompiler { get; set; } = false;

    public void AddDbContext<TDbContext>(Action<ADTOSharpDbContextConfiguration<TDbContext>> action)
        where TDbContext : DbContext
    {
        _iocManager.IocContainer.Register(
            Component.For<IADTOSharpDbContextConfigurer<TDbContext>>().Instance(
                new ADTOSharpDbContextConfigurerAction<TDbContext>(action)
            ).IsDefault()
        );
    }
}