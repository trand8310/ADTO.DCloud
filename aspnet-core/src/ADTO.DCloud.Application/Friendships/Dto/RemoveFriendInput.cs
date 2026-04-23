using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ADTO.DCloud.Friendships.Dto
{
    public class RemoveFriendInput
    {
        public Guid UserId { get; set; }

        public Guid? TenantId { get; set; }
    }
}
