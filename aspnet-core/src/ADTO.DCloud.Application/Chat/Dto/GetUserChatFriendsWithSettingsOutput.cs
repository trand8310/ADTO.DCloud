using System;
using System.Collections.Generic;
using ADTO.DCloud.Friendships.Dto;
using Castle.Components.DictionaryAdapter;


namespace ADTO.DCloud.Chat.Dto
{
    public class GetUserChatFriendsWithSettingsOutput
    {
        public DateTime ServerTime { get; set; }
        
        public List<FriendDto> Friends { get; set; }

        public GetUserChatFriendsWithSettingsOutput()
        {
            Friends = new EditableList<FriendDto>();
        }
    }
}