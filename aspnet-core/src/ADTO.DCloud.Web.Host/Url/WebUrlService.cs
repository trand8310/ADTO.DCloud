using ADTO.DCloud.Configuration;
using ADTO.DCloud.Url;
using ADTOSharp.Dependency;

namespace ADTO.DCloud.Web.Url
{
    public class WebUrlService : WebUrlServiceBase, IWebUrlService, ITransientDependency
    {
        public WebUrlService(
            IAppConfigurationAccessor configurationAccessor) :
            base(configurationAccessor)
        {
        }
        /// <summary>
        /// 前端访问地址
        /// </summary>
        public override string WebSiteRootAddressFormatKey => "App:ClientRootAddress";
        /// <summary>
        /// 接口服务地址
        /// </summary>
        public override string ServerRootAddressFormatKey => "App:ServerRootAddress";
    }
}