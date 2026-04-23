using System;

namespace ADTO.DCloud.Chat.Dto
{
    public class ChatMessageExportDto
    {
        public Guid TargetUserId { get; set; }

        public string TargetUserName { get; set; }

        public string TargetTenantName { get; set; }

        public Guid? TargetTenantId { get; set; }

        public ChatSide Side { get; set; }

        public ChatMessageReadState ReadState { get; set; }

        public ChatMessageReadState ReceiverReadState { get; set; }

        public string Message { get; set; }

        public DateTime CreationTime { get; set; }
    }
}