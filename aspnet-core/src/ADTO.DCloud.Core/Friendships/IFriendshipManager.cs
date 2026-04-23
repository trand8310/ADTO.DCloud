using ADTOSharp;
using ADTOSharp.Domain.Services;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace ADTO.DCloud.Friendships
{
    public interface IFriendshipManager : IDomainService
    {
        Task CreateFriendshipAsync(Friendship friendship);

        Task UpdateFriendshipAsync(Friendship friendship);

        Task<Friendship> GetFriendshipOrNullAsync(UserIdentifier user, UserIdentifier probableFriend);

        Task BanFriendAsync(UserIdentifier userIdentifier, UserIdentifier probableFriend);
        Task RemoveFriendAsync(UserIdentifier userIdentifier, UserIdentifier probableFriend);

        Task AcceptFriendshipRequestAsync(UserIdentifier userIdentifier, UserIdentifier probableFriend);

        Task<List<Friendship>> GetAllFriendshipsAsync(UserIdentifier user);
    }
}
