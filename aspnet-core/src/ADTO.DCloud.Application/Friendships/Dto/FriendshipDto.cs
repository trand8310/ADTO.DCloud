using ADTO.DCloud.Friendships.Cache;
using ADTOSharp.AutoMapper;
using System;

namespace ADTO.DCloud.Friendships.Dto
{
 
    public class FriendDto
    {
        public Guid FriendUserId { get; set; }

        public Guid? FriendTenantId { get; set; }

        public string FriendUserName { get; set; }

        public string FriendTenancyName { get; set; }

        public Guid? FriendProfilePictureId { get; set; }

        public int UnreadMessageCount { get; set; }

        public bool IsOnline { get; set; }

        public FriendshipState State { get; set; }
    }
}
