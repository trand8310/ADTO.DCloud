using ADTOSharp.Authorization.Users;
using ADTOSharp.AutoMapper;
using System;

namespace ADTO.DCloud.Authorization.Users.Dto
{
    [AutoMap(typeof(UserLoginAttempt))]
    public class UserLoginAttemptDto
    {
        /// <summary>
        /// 租户名称
        /// </summary>
        public string TenancyName { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 客户端IP
        /// </summary>
        public string ClientIpAddress { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        public string ClientName { get; set; }
        /// <summary>
        /// 浏览器信息
        /// </summary>
        public string BrowserInfo { get; set; }
        /// <summary>
        /// 结果
        /// </summary>
        public string Result { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }
    }
}
