using System.Threading.Tasks;


namespace ADTO.DCloud.WebHooks
{
    public interface IAppWebhookPublisher
    {
        Task PublishTestWebhook();
    }
}
