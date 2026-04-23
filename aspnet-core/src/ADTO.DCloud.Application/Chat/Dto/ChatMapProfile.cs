
using ADTO.DCloud.Friendships;
using ADTO.DCloud.Friendships.Cache;
using ADTO.DCloud.Friendships.Dto;

namespace ADTO.DCloud.Chat.Dto
{
    public class ChatMapProfile : AutoMapper.Profile
    {
        public ChatMapProfile()
        {
            CreateMap<FriendDto, FriendCacheItem>();
            CreateMap<FriendDto, Friendship>();

            CreateMap<FriendCacheItem,FriendDto>();
            CreateMap<Friendship,FriendDto>();
        }
    }
}
