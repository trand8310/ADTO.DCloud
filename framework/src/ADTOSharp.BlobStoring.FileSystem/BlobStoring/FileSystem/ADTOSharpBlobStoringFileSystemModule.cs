using System.Reflection;
using ADTOSharp.Modules;

namespace ADTOSharp.BlobStoring.FileSystem
{
    [DependsOn(typeof(ADTOSharpBlobStoringModule))]
    public class ADTOSharpBlobStoringFileSystemModule : ADTOSharpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
