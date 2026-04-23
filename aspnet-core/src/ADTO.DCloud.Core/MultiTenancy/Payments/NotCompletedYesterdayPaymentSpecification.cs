using System;
using System.Linq.Expressions;
using ADTOSharp.Specifications;
using ADTOSharp.Timing;

namespace ADTO.DCloud.MultiTenancy.Payments
{
    public class NotCompletedYesterdayPaymentSpecification: Specification<SubscriptionPayment>
    {
        public override Expression<Func<SubscriptionPayment, bool>> ToExpression()
        {
            var todaysDate = Clock.Now.Date;
            var yesterdaysDate = todaysDate.AddDays(-1).Date;

            return payment =>
                payment.Status == SubscriptionPaymentStatus.NotPaid &&
                payment.CreationTime >= yesterdaysDate &&
                payment.CreationTime < todaysDate;
        }
    }
}