using System.Threading.Tasks;
using ADTOSharp.Application.Services;

namespace ADTO.DCloud.MultiTenancy
{
    public interface ISubscriptionAppService : IApplicationService
    {
        Task DisableRecurringPayments();

        Task EnableRecurringPayments();
    }
}
