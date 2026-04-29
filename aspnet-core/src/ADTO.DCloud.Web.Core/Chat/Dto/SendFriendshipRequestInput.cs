using System;

namespace ADTO.DCloud.Chat.Dto
{
    public class SendFriendshipRequestInput
    {
        public Guid UserId { get; set; }

        public Guid? TenantId { get; set; }
    }
}