using ADTO.DCloud.Friendships;
using ADTOSharp.Application.Services.Dto;
using System;



namespace ADTO.DCloud.Chat.Dto
{
    public class ChatUserDto : EntityDto<Guid>
    {
        public Guid? TenantId { get; set; }

        public Guid? ProfilePictureId { get; set; }

        public string UserName { get; set; }

        public string TenancyName { get; set; }

        public int UnreadMessageCount { get; set; }

        public bool IsOnline { get; set; }

        public FriendshipState State { get; set; }
    }
}