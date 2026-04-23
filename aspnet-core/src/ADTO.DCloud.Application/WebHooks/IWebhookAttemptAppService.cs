using System.Threading.Tasks;
using ADTOSharp.Application.Services.Dto;
using ADTO.DCloud.WebHooks.Dto;

namespace ADTO.DCloud.WebHooks;

public interface IWebhookAttemptAppService
{
    Task<PagedResultDto<GetAllSendAttemptsOutput>> GetAllSendAttempts(GetAllSendAttemptsInput input);

    Task<ListResultDto<GetAllSendAttemptsOfWebhookEventOutput>> GetAllSendAttemptsOfWebhookEvent(GetAllSendAttemptsOfWebhookEventInput input);
}
