using System;

namespace ADTO.DCloud.Web.Chat.SignalR
{
    public class SendChatMessageInput
    {
        /// <summary>
        /// 租户ID
        /// </summary>
        public Guid? TenantId { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 租户名称
        /// </summary>
        public string TenancyName { get; set; }
        /// <summary>
        /// 用户图像
        /// </summary>
        public Guid? ProfilePictureId { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }
    }
}