using ADTO.DCloud.EntityFrameworkCore.Seed;
using ADTO.DCloud.Net.Emailing;
using ADTO.OpenIddict.EntityFrameworkCore;
using ADTOSharp.Configuration.Startup;
using ADTOSharp.Dependency;
using ADTOSharp.Domain.Uow;
using ADTOSharp.EntityFrameworkCore.Configuration;
using ADTOSharp.MailKit;
using ADTOSharp.Modules;
using ADTOSharp.Reflection.Extensions;
using ADTOSharp.Snowflakes;


namespace ADTO.DCloud.EntityFrameworkCore;

[DependsOn(
    typeof(ADTOSharpSnowflakesModule),
    typeof(DCloudCoreModule),
    typeof(ADTOOpenIddictEntityFrameworkCoreModule))]
public class DCloudEntityFrameworkModule : ADTOSharpModule
{
    /* Used it tests to skip dbcontext registration, in order to use in-memory database of EF Core */
    public bool SkipDbContextRegistration { get; set; }

    public bool SkipDbSeed { get; set; }

    public override void PreInitialize()
    {
        Configuration.ReplaceService<IConnectionStringResolver, ReadWriteConnectionStringResolver>(DependencyLifeStyle.Transient);
        if (!SkipDbContextRegistration)
        {
            Configuration.Modules.ADTOSharpEfCore().AddDbContext<DCloudDbContext>(options =>
            {
                if (options.ExistingConnection != null)
                {
                    DCloudDbContextConfigurer.Configure(options.DbContextOptions, options.ExistingConnection);
                }
                else
                {
                    DCloudDbContextConfigurer.Configure(options.DbContextOptions, options.ConnectionString);
                }
            });
        }
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(DCloudEntityFrameworkModule).GetAssembly());
    }

    public override void PostInitialize()
    {
        if (!SkipDbSeed)
        {
            SeedHelper.SeedHostDb(IocManager);
        }
    }
}

