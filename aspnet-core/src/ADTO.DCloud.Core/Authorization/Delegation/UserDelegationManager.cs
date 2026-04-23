using System;
using System.Threading.Tasks;
using ADTOSharp;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Domain.Uow;

namespace ADTO.DCloud.Authorization.Delegation;
/// <summary>
/// 用户代理管理
/// </summary>
public class UserDelegationManager : DCloudServiceBase, IUserDelegationManager
{
    private readonly IRepository<UserDelegation, Guid> _userDelegationRepository;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public UserDelegationManager(IRepository<UserDelegation, Guid> userDelegationRepository, IUnitOfWorkManager unitOfWorkManager)
    {
        _userDelegationRepository = userDelegationRepository;
        _unitOfWorkManager = unitOfWorkManager;
    }

    public async Task<bool> HasActiveDelegationAsync(Guid sourceUserId, Guid targetUserId)
    {
        var activeUserDelegationExpression = new ActiveUserDelegationSpecification(sourceUserId, targetUserId)
            .ToExpression();

        var activeDelegation = await _userDelegationRepository.FirstOrDefaultAsync(activeUserDelegationExpression);

        return activeDelegation != null;
    }

    public bool HasActiveDelegation(Guid sourceUserId, Guid targetUserId)
    {
        UserDelegation activeDelegation;
        using (var uow = _unitOfWorkManager.Begin())
        {
            var activeUserDelegationExpression = new ActiveUserDelegationSpecification(sourceUserId, targetUserId)
                .ToExpression();

            activeDelegation = _userDelegationRepository.FirstOrDefault(activeUserDelegationExpression);
            uow.Complete();
        }

        return activeDelegation != null;
    }

    public async Task RemoveDelegationAsync(Guid userDelegationId, UserIdentifier currentUser)
    {
        var delegation = await _userDelegationRepository.FirstOrDefaultAsync(e =>
            e.Id == userDelegationId && e.SourceUserId == currentUser.UserId
        );

        if (delegation == null)
        {
            throw new Exception("Only source user can delete a user delegation !");
        }

        await _userDelegationRepository.DeleteAsync(delegation);
    }

    public async Task<UserDelegation> GetAsync(Guid userDelegationId)
    {
        return await _userDelegationRepository.GetAsync(userDelegationId);
    }
}
