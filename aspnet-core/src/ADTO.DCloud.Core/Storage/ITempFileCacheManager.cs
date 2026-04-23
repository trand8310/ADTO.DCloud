using ADTOSharp.Dependency;
using System.Threading.Tasks;

namespace ADTO.DCloud.Storage
{
    public interface ITempFileCacheManager : ITransientDependency
    {
        void SetFile(string token, byte[] content);
        Task SetFileAsync(string token, TempFileInfo info);
        Task SetFileAsync(string token, byte[] content);



        byte[] GetFile(string token);

        void SetFile(string token, TempFileInfo info);

        TempFileInfo GetFileInfo(string token);
    }
}