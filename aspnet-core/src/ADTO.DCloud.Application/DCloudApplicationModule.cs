using ADTO.DCloud.Authorization;
using ADTO.DCloud.Infrastructure;
using ADTO.DCloud.Media.FileManage;
using ADTO.DCloud.Media.FileManage.Aliyun;
using ADTO.DCloud.Storage;
using ADTO.DCloud.Url;
using ADTOSharp;
using ADTOSharp.AutoMapper;
using ADTOSharp.Modules;
using ADTOSharp.Reflection.Extensions;


namespace ADTO.DCloud;

[DependsOn(
    typeof(DCloudCoreModule),
    typeof(ADTOSharpAutoMapperModule))]
public class DCloudApplicationModule : ADTOSharpModule
{
    public override void PreInitialize()
    {
 
        Configuration.Authorization.Providers.Add<DCloudAuthorizationProvider>();
        //自定义映射入口
        Configuration.Modules.ADTOSharpAutoMapper().Configurators.Add(CustomDtoMapper.CreateMappings);
       
        IocManager.IocContainer.Register(
            Castle.MicroKernel.Registration.Component
                .For<IAliyunFileService, IFileManageService>()
                .ImplementedBy<AliyunFileService>()
                .LifestyleTransient()
        );

        IocManager.Register<IUrlRecordService, UrlRecordService>();
        IocManager.Register<IDownloadService, DownloadService>();



    }

    public override void Initialize()
    {
        var thisAssembly = typeof(DCloudApplicationModule).GetAssembly();

        IocManager.RegisterAssemblyByConvention(thisAssembly);

        //扫描程序集查找从AutoMapper继承的类,可以实现DTO与实体之间的相互映射
        Configuration.Modules.ADTOSharpAutoMapper().Configurators.Add(
            cfg => cfg.AddMaps(thisAssembly)
        );
    }
}

