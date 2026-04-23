using System;
using System.Collections.Generic;

namespace ADTO.DCloud.Friendships.Cache
{
    public class UserWithFriendsCacheItem
    {
        public Guid? TenantId { get; set; }

        public Guid UserId { get; set; }

        public string TenancyName { get; set; }

        public string UserName { get; set; }

        public Guid? ProfilePictureId { get; set; }

        public List<FriendCacheItem> Friends { get; set; }
    }
}