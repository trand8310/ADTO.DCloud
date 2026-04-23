using System;
using System.ComponentModel.DataAnnotations;

namespace ADTO.DCloud.Friendships.Dto
{
    public class CreateFriendshipRequestInput
    {
        public Guid UserId { get; set; }

        public Guid? TenantId { get; set; }
    }
}