using ADTOSharp;
using System;

namespace ADTO.DCloud.Authorization.Users.Dto
{
    public class UnlinkUserInput
    {
        public Guid? TenantId { get; set; }

        public Guid UserId { get; set; }

        public UserIdentifier ToUserIdentifier()
        {
            return new UserIdentifier(TenantId, UserId);
        }
    }
}