using ADTO.DCloud.Chat.Dto;
using ADTOSharp.Application.Services;
using ADTOSharp.Application.Services.Dto;
using System.Threading.Tasks;
 

namespace ADTO.DCloud.Chat
{
    public interface IChatAppService : IApplicationService
    {
        Task<GetUserChatFriendsWithSettingsOutput> GetUserChatFriendsWithSettings();

        Task<ListResultDto<ChatMessageDto>> GetUserChatMessages(GetUserChatMessagesInput input);

        Task MarkAllUnreadMessagesOfUserAsRead(MarkAllUnreadMessagesOfUserAsReadInput input);



    }
}
