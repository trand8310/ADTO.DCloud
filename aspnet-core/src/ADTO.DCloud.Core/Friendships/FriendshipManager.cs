using ADTOSharp;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Domain.Uow;
using ADTOSharp.UI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace ADTO.DCloud.Friendships;

/// <summary>
/// 好友管理
/// </summary>
public class FriendshipManager : DCloudDomainServiceBase, IFriendshipManager
{
    private readonly IRepository<Friendship, Guid> _friendshipRepository;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public FriendshipManager(
        IRepository<Friendship, Guid> friendshipRepository,
        IUnitOfWorkManager unitOfWorkManager)
    {
        _friendshipRepository = friendshipRepository;
        _unitOfWorkManager = unitOfWorkManager;
    }

    public async Task CreateFriendshipAsync(Friendship friendship)
    {
        await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
        {
            if (friendship.TenantId == friendship.FriendTenantId &&
                friendship.UserId == friendship.FriendUserId)
            {
                throw new UserFriendlyException(L("YouCannotBeFriendWithYourself"));
            }

            using (CurrentUnitOfWork.SetTenantId(friendship.TenantId))
            {
                await _friendshipRepository.InsertAsync(friendship);
                await CurrentUnitOfWork.SaveChangesAsync();
            }
        });
    }

    public async Task UpdateFriendshipAsync(Friendship friendship)
    {
        await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
        {
            using (CurrentUnitOfWork.SetTenantId(friendship.TenantId))
            {
                await _friendshipRepository.UpdateAsync(friendship);
                await CurrentUnitOfWork.SaveChangesAsync();
            }
        });
    }

    public async Task<Friendship> GetFriendshipOrNullAsync(UserIdentifier user, UserIdentifier probableFriend)
    {
        return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
        {
            using (CurrentUnitOfWork.SetTenantId(user.TenantId))
            {
                return await _friendshipRepository.FirstOrDefaultAsync(friendship =>
                    friendship.UserId == user.UserId &&
                    friendship.TenantId == user.TenantId &&
                    friendship.FriendUserId == probableFriend.UserId &&
                    friendship.FriendTenantId == probableFriend.TenantId);
            }
        });
    }

    public async Task BanFriendAsync(UserIdentifier userIdentifier, UserIdentifier probableFriend)
    {
        await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
        {
            var friendship = (await GetFriendshipOrNullAsync(userIdentifier, probableFriend));
            if (friendship == null)
            {
                throw new Exception("Friendship does not exist between " + userIdentifier + " and " + probableFriend);
            }

            friendship.State = FriendshipState.Blocked;
            await UpdateFriendshipAsync(friendship);
        });
    }

    public async Task RemoveFriendAsync(UserIdentifier userIdentifier, UserIdentifier probableFriend)
    {
        await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
        {
            var friendship = (await GetFriendshipOrNullAsync(userIdentifier, probableFriend));
            if (friendship == null)
            {
                throw new Exception("Friendship does not exist between " + userIdentifier + " and " + probableFriend);
            }

            await _friendshipRepository.DeleteAsync(friendship);
        });
    }

    public async Task AcceptFriendshipRequestAsync(UserIdentifier userIdentifier, UserIdentifier probableFriend)
    {
        await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
        {
            var friendship = (await GetFriendshipOrNullAsync(userIdentifier, probableFriend));
            if (friendship == null)
            {
                throw new Exception("Friendship does not exist between " + userIdentifier + " and " + probableFriend);
            }

            friendship.State = FriendshipState.Accepted;
            await UpdateFriendshipAsync(friendship);
        });
    }

    public async Task<List<Friendship>> GetAllFriendshipsAsync(UserIdentifier user)
    {
        return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
        {
            using (CurrentUnitOfWork.SetTenantId(user.TenantId))
            {
                return await _friendshipRepository.GetAllListAsync(friendship =>
                    friendship.UserId == user.UserId &&
                    friendship.TenantId == user.TenantId && 
                    friendship.State == FriendshipState.Accepted);
            }
        });
    }
}

