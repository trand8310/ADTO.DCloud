

using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace ADTO.DCloud.Storage
{
    public class DownloadService : IDownloadService
    {
        public virtual async Task<byte[]> GetDownloadBitsAsync(IFormFile file)
        {
            await using var fileStream = file.OpenReadStream();
            await using var ms = new MemoryStream();
            await fileStream.CopyToAsync(ms);
            var fileBytes = ms.ToArray();
            return fileBytes;
        }
    }
}
