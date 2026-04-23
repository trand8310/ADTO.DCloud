namespace ADTOSharp.Web.Models
{
    public abstract class AjaxResponseBase
    {
        /// <summary>
        /// 此属性可用于将用户重定向到指定的URL。
        /// </summary>
        public string TargetUrl { get; set; }

        /// <summary>
        /// 显示执行结果的成功状态。
        /// Set <see cref="Error"/> if this value is false.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 错误详情 (Must and only set if <see cref="Success"/> is false).
        /// </summary>
        public ErrorInfo Error { get; set; }

        /// <summary>
        /// 此属性可用于指示当前用户没有执行此请求的权限。
        /// </summary>
        public bool UnAuthorizedRequest { get; set; }
    }
}