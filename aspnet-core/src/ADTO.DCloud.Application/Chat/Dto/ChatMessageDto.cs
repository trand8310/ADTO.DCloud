using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;


namespace ADTO.DCloud.Chat.Dto
{
    [AutoMap(typeof(ChatMessage))]
    public class ChatMessageDto : EntityDto<long>
    {
        public Guid UserId { get; set; }

        public Guid? TenantId { get; set; }

        public Guid TargetUserId { get; set; }

        public Guid? TargetTenantId { get; set; }

        public ChatSide Side { get; set; }

        public ChatMessageReadState ReadState { get; set; }

        public ChatMessageReadState ReceiverReadState { get; set; }

        public string Message { get; set; }
        
        public DateTime CreationTime { get; set; }

        public Guid SharedMessageId { get; set; }
    }
}