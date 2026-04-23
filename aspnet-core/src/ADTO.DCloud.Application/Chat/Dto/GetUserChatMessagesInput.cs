using System;
 
namespace ADTO.DCloud.Chat.Dto
{
    public class GetUserChatMessagesInput
    {
        public Guid? TenantId { get; set; }

        public Guid UserId { get; set; }

        public long? MinMessageId { get; set; }

        public int MaxResultCount { get; set; } = 10;
    }
}