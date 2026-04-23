using ADTO.DCloud.MultiTenancy.Payments.Stripe;

namespace ADTO.DCloud.Web.Controllers;

public class StripeController : StripeControllerBase
{
    public StripeController(
        StripeGatewayManager stripeGatewayManager,
        StripePaymentGatewayConfiguration stripeConfiguration,
        IStripePaymentAppService stripePaymentAppService) 
        : base(stripeGatewayManager, stripeConfiguration, stripePaymentAppService)
    {
    }
}
