using Microsoft.Extensions.Configuration;
using Castle.MicroKernel.Registration;
using ADTOSharp.Events.Bus;
using ADTOSharp.Modules;
using ADTOSharp.Reflection.Extensions;
using ADTO.DCloud.Configuration;
using ADTO.DCloud.EntityFrameworkCore;
using ADTO.DCloud.Migrator.DependencyInjection;

namespace ADTO.DCloud.Migrator
{
    [DependsOn(typeof(DCloudEntityFrameworkModule))]
    public class DCloudMigratorModule : ADTOSharpModule
    {
        private readonly IConfigurationRoot _appConfiguration;

        public DCloudMigratorModule(DCloudEntityFrameworkModule adtosharpProjectNameEntityFrameworkModule)
        {
            adtosharpProjectNameEntityFrameworkModule.SkipDbSeed = true;

            _appConfiguration = AppConfigurations.Get(
                typeof(DCloudMigratorModule).GetAssembly().GetDirectoryPathOrNull()
            );
        }

        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
                DCloudConsts.ConnectionStringName
            );

            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
            Configuration.ReplaceService(
                typeof(IEventBus), 
                () => IocManager.IocContainer.Register(
                    Component.For<IEventBus>().Instance(NullEventBus.Instance)
                )
            );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(DCloudMigratorModule).GetAssembly());
            ServiceCollectionRegistrar.Register(IocManager);
        }
    }
}

