using System.Collections.Generic;
using System.Threading.Tasks;
using ADTOSharp.Application.Services;
using ADTO.DCloud.MultiTenancy.Dto;
using ADTO.DCloud.MultiTenancy.Payments.Dto;
using ADTOSharp.Application.Services.Dto;
using System;

namespace ADTO.DCloud.MultiTenancy.Payments
{
    public interface IPaymentAppService : IApplicationService
    {
        Task<PaymentInfoDto> GetPaymentInfo(PaymentInfoInput input);

        Task<long> CreatePayment(CreatePaymentDto input);

        Task CancelPayment(CancelPaymentDto input);

        Task<PagedResultDto<SubscriptionPaymentListDto>> GetPaymentHistory(GetPaymentHistoryInput input);

        List<PaymentGatewayModel> GetActiveGateways(GetActiveGatewaysInput input);

        Task<SubscriptionPaymentDto> GetPaymentAsync(long paymentId);

        Task<SubscriptionPaymentDto> GetLastCompletedPayment();

        Task BuyNowSucceed(long paymentId);

        Task NewRegistrationSucceed(long paymentId);

        Task UpgradeSucceed(long paymentId);

        Task ExtendSucceed(long paymentId);

        Task PaymentFailed(long paymentId);

        Task SwitchBetweenFreeEditions(Guid upgradeEditionId);

        Task UpgradeSubscriptionCostsLessThenMinAmount(Guid editionId);

        Task<bool> HasAnyPayment();
    }
}
