using Microsoft.Extensions.DependencyInjection;
using ADTO.DCloud.HealthChecks;

namespace ADTO.DCloud.Web.HealthCheck
{
    public static class ADTOSharpHealthCheck
    {
        /// <summary>
        /// ADTOSharpHealthCheck
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IHealthChecksBuilder AddADTOSharpHealthCheck(this IServiceCollection services)
        {
            var builder = services.AddHealthChecks();
            builder.AddCheck<DCloudDbContextHealthCheck>("Database Connection");
            builder.AddCheck<DCloudDbContextUsersHealthCheck>("Database Connection with user check");
            builder.AddCheck<CacheHealthCheck>("Cache");

            // add your custom health checks here
            // builder.AddCheck<MyCustomHealthCheck>("my health check");

            return builder;
        }
    }
}

