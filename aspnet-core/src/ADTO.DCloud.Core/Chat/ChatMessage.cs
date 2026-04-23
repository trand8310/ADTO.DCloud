using ADTOSharp;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using ADTOSharp.Timing;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ADTO.DCloud.Chat
{
    /// <summary>
    /// 消息表,用户-用户,系统-用户之间相互发送的消息
    /// </summary>
    [Table("ChatMessages"),Description("消息")]
    public class ChatMessage : Entity<long>, IHasCreationTime, IMayHaveTenant
    {
        public const int MaxMessageLength = 4 * 1024; //4KB

        /// <summary>
        /// 用户Id
        /// </summary>
        [Description("用户Id")]
        public Guid UserId { get; set; }
        /// <summary>
        /// 租户Id
        /// </summary>
        [Description("租户Id")]
        public Guid? TenantId { get; set; }
        /// <summary>
        /// 目标用户Id
        /// </summary>
        [Description("目标用户Id")]
        public Guid TargetUserId { get; set; }
        /// <summary>
        /// 目标租户Id
        /// </summary>
        [Description("目标租户Id")]
        public Guid? TargetTenantId { get; set; }

        /// <summary>
        /// 发送的消息
        /// </summary>
        [Description("发送的消息")]
        [Required]
        [StringLength(MaxMessageLength)]
        public string Message { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Description("创建时间")]
        public DateTime CreationTime { get; set; }
        /// <summary>
        /// 发送方向
        /// </summary>
        [Description("发送方向")]
        public ChatSide Side { get; set; }

        /// <summary>
        /// 阅读状态
        /// </summary>
        [Description("阅读状态")]
        public ChatMessageReadState ReadState { get; private set; }
        /// <summary>
        /// 接受人阅读状态
        /// </summary>
        [Description("接受人阅读状态")]
        public ChatMessageReadState ReceiverReadState { get; private set; }

        /// <summary>
        /// 共享消息Id
        /// </summary>
        [Description("共享消息Id")]
        public Guid? SharedMessageId { get; set; }

        public ChatMessage(
            UserIdentifier user,
            UserIdentifier targetUser,
            ChatSide side,
            string message,
            ChatMessageReadState readState,
            Guid sharedMessageId,
            ChatMessageReadState receiverReadState)
        {
            UserId = user.UserId;
            TenantId = user.TenantId;
            TargetUserId = targetUser.UserId;
            TargetTenantId = targetUser.TenantId;
            Message = message;
            Side = side;
            ReadState = readState;
            SharedMessageId = sharedMessageId;
            ReceiverReadState = receiverReadState;
            CreationTime = Clock.Now;
        }

        public void ChangeReadState(ChatMessageReadState newState)
        {
            ReadState = newState;
        }

        protected ChatMessage()
        {

        }

        public void ChangeReceiverReadState(ChatMessageReadState newState)
        {
            ReceiverReadState = newState;
        }
    }
}
