using ADTOSharp;
using ADTOSharp.Domain.Services;
using System;
using System.Threading.Tasks;


namespace ADTO.DCloud.Chat
{
    public interface IChatMessageManager : IDomainService
    {
        Task SendMessageAsync(UserIdentifier sender, UserIdentifier receiver, string message, string senderTenancyName, string senderUserName, Guid? senderProfilePictureId);

        long Save(ChatMessage message);

        int GetUnreadMessageCount(UserIdentifier userIdentifier, UserIdentifier sender);

        Task<ChatMessage> FindMessageAsync(long id, Guid userId);
    }
}
