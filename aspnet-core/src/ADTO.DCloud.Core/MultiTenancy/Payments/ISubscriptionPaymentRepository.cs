using System;
using System.Threading.Tasks;
using ADTOSharp.Domain.Repositories;

namespace ADTO.DCloud.MultiTenancy.Payments
{
    public interface ISubscriptionPaymentRepository : IRepository<SubscriptionPayment, long>
    {
        Task<SubscriptionPayment> GetByGatewayAndPaymentIdAsync(SubscriptionPaymentGatewayType gateway, string paymentId);

        Task<SubscriptionPayment> GetLastCompletedPaymentOrDefaultAsync(Guid tenantId, SubscriptionPaymentGatewayType? gateway, bool? isRecurring);

        Task<SubscriptionPayment> GetLastPaymentOrDefaultAsync(Guid tenantId, SubscriptionPaymentGatewayType? gateway, bool? isRecurring);
    }
}
