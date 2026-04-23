using ADTOSharp.Authorization.Users;
using ADTOSharp.Dependency;
using ADTOSharp.Localization.Dictionaries.Xml;
using ADTOSharp.Localization.Sources;
using ADTOSharp.Modules;
using ADTOSharp.Reflection.Extensions;
using ADTOSharp.Threading.BackgroundWorkers;
using ADTOSharp.Zero.Configuration;

namespace ADTOSharp.Zero;

[DependsOn(typeof(ADTOSharpZeroCommonModule))]
public class ADTOSharpZeroCoreModule : ADTOSharpModule
{
    public override void PreInitialize()
    {
        Configuration.Localization.Sources.Extensions.Add(
            new LocalizationSourceExtensionInfo(
                ADTOSharpZeroConsts.LocalizationSourceName,
                new XmlEmbeddedFileLocalizationDictionaryProvider(
                    typeof(ADTOSharpZeroCoreModule).GetAssembly(), "ADTOSharp.Zero.Localization.SourceExt"
                )
            )
        );
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(ADTOSharpZeroCoreModule).GetAssembly());
        RegisterUserTokenExpirationWorker();
    }

    public override void PostInitialize()
    {
        if (Configuration.BackgroundJobs.IsJobExecutionEnabled)
        {
            using (var entityTypes = IocManager.ResolveAsDisposable<IADTOSharpZeroEntityTypes>())
            {
                var implType = typeof(UserTokenExpirationWorker<,>)
                    .MakeGenericType(entityTypes.Object.Tenant, entityTypes.Object.User);
                var workerManager = IocManager.Resolve<IBackgroundWorkerManager>();
                workerManager.Add(IocManager.Resolve(implType) as IBackgroundWorker);
            }
        }
    }

    private void RegisterUserTokenExpirationWorker()
    {
        using (var entityTypes = IocManager.ResolveAsDisposable<IADTOSharpZeroEntityTypes>())
        {
            var implType = typeof(UserTokenExpirationWorker<,>)
                .MakeGenericType(entityTypes.Object.Tenant, entityTypes.Object.User);
            IocManager.Register(implType);
        }
    }
}