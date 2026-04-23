using ADTOSharp;
using System;

namespace ADTO.DCloud.Chat.Dto
{
    public class MarkAllUnreadMessagesOfUserAsReadInput
    {
        public Guid? TenantId { get; set; }

        public Guid UserId { get; set; }

        public UserIdentifier ToUserIdentifier()
        {
            return new UserIdentifier(TenantId, UserId);
        }
    }
}