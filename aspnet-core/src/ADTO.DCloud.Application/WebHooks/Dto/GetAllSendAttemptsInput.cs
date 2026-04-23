using ADTO.DCloud.Dto;

namespace ADTO.DCloud.WebHooks.Dto;

public class GetAllSendAttemptsInput : PagedInputDto
{
    public string SubscriptionId { get; set; }
}
