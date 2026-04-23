

using ADTO.DCloud.Dto;
using ADTO.DCloud.Logging.Dto;
using ADTOSharp.Application.Services;

namespace ADTO.DCloud.Logging
{
    public interface IWebLogAppService : IApplicationService
    {
        GetLatestWebLogsOutput GetLatestWebLogs();

        FileDto DownloadWebLogs();
    }
}
