using System.Threading.Tasks;
using ADTOSharp.Application.Services;
using ADTO.DCloud.MultiTenancy.Payments.Dto;
using ADTO.DCloud.MultiTenancy.Payments.Stripe.Dto;

namespace ADTO.DCloud.MultiTenancy.Payments.Stripe
{
    public interface IStripePaymentAppService : IApplicationService
    {
        Task ConfirmPayment(StripeConfirmPaymentInput input);

        StripeConfigurationDto GetConfiguration();

        Task<SubscriptionPaymentDto> GetPaymentAsync(StripeGetPaymentInput input);

        Task<string> CreatePaymentSession(StripeCreatePaymentSessionInput input);
    }
}