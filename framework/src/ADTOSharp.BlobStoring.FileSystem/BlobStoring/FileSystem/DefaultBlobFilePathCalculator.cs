using System.IO;
using ADTOSharp.Dependency;
using ADTOSharp.Runtime.Session;

namespace ADTOSharp.BlobStoring.FileSystem
{
    public class DefaultBlobFilePathCalculator : IBlobFilePathCalculator, ITransientDependency
    {
        protected IADTOSharpSession ADTOSharpSession { get; }

        public DefaultBlobFilePathCalculator(IADTOSharpSession session)
        {
            ADTOSharpSession = session;
        }

        public virtual string Calculate(BlobProviderArgs args)
        {
            var fileSystemConfiguration = args.Configuration.GetFileSystemConfiguration();
            var blobPath = fileSystemConfiguration.BasePath;

            blobPath = ADTOSharpSession.TenantId == null
                ? Path.Combine(blobPath, "host")
                : Path.Combine(blobPath, "tenants", ADTOSharpSession.TenantId.Value.ToString("D"));

            if (fileSystemConfiguration.AppendContainerNameToBasePath)
            {
                blobPath = Path.Combine(blobPath, args.ContainerName);
            }

            blobPath = Path.Combine(blobPath, args.BlobName);

            return blobPath;
        }
    }
}
