using ADTOSharp.AutoMapper;
using System;

namespace ADTO.DCloud.Friendships.Cache
{
    [AutoMapFrom(typeof(Friendship))]
    public class FriendCacheItem
    {
        public const string CacheName = "ADTOSharpUserFriendCache";

        public Guid FriendUserId { get; set; }

        public Guid? FriendTenantId { get; set; }

        public string FriendUserName { get; set; }

        public string FriendTenancyName { get; set; }

        public Guid? FriendProfilePictureId { get; set; }

        public int UnreadMessageCount { get; set; }

        public FriendshipState State { get; set; }
    }
}