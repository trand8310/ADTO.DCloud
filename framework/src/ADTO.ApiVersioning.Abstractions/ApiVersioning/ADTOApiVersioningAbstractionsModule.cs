
using ADTOSharp.Dependency;
using ADTOSharp.Modules;
using Castle.MicroKernel.Registration;

namespace ADTO.ApiVersioning;

public class ADTOApiVersioningAbstractionsModule : ADTOSharpModule
{
    public override void PreInitialize()
    {
        IocManager.IocContainer.Register(Component.For<IRequestedApiVersion>()
         .Instance(NullRequestedApiVersion.Instance)
         .LifestyleSingleton());
    }



}
