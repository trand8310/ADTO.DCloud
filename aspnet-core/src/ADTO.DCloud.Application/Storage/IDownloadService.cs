

using ADTOSharp.Application.Services;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ADTO.DCloud.Storage
{
    public partial interface IDownloadService
    {
        Task<byte[]> GetDownloadBitsAsync(IFormFile file);
    }
}
