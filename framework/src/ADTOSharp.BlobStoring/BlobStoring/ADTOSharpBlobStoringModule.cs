using System.Reflection;
using ADTOSharp.Dependency;
using ADTOSharp.Modules;

namespace ADTOSharp.BlobStoring
{
    public class ADTOSharpBlobStoringModule : ADTOSharpModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<ADTOSharpBlobStoringOptions>();

            IocManager.Register(typeof(IBlobContainer<>), typeof(BlobContainer<>), DependencyLifeStyle.Transient);
            IocManager.Register<IBlobContainer, BlobContainer<DefaultContainer>>(DependencyLifeStyle.Transient);

        }
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
