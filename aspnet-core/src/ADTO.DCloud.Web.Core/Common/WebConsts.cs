using System.Collections.Generic;

namespace ADTO.DCloud.Web.Common
{
    public static class WebConsts
    {
        public const string SwaggerUiEndPoint = "/swagger";
        public const string HangfireDashboardEndPoint = "/hangfire";

        public static bool SwaggerUiEnabled = false;
        public static bool HangfireDashboardEnabled = false;

        public static List<string> ReCaptchaIgnoreWhiteList = new List<string>
        {
           DCloudConsts.ApiClientUserAgent
        };

        public static class GraphQL
        {
            public const string PlaygroundEndPoint = "/ui/playground";
            public const string EndPoint = "/graphql";

            public static bool PlaygroundEnabled = false;
            public static bool Enabled = false;
        }
    }
}

