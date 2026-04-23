using System;

namespace ADTO.DCloud.Web.Chat.SignalR
{
    public class SendFriendshipRequestInput
    {
        public Guid UserId { get; set; }

        public Guid? TenantId { get; set; }
    }
}