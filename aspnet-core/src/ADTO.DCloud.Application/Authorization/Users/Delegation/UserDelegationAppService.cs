using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Authorization;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Linq.Extensions;
using ADTOSharp.Runtime.Session;
using ADTOSharp.Timing;
using ADTOSharp.UI;
using Microsoft.EntityFrameworkCore;
using ADTO.DCloud.Authorization.Delegation;
using ADTO.DCloud.Authorization.Users.Delegation.Dto;

namespace ADTO.DCloud.Authorization.Users.Delegation;

/// <summary>
///  用户委托服务,当前用户,委托同租户的其它用户行使自已的在系统中的功能.
///  这个功能,在审批流程,有用,具体的实现在写这个服务的时候,还在思索....
/// </summary>
[ADTOSharpAuthorize]
public class UserDelegationAppService : DCloudAppServiceBase, IUserDelegationAppService
{
    private readonly IRepository<UserDelegation, Guid> _userDelegationRepository;
    private readonly IRepository<User, Guid> _userRepository;
    private readonly IUserDelegationManager _userDelegationManager;
    private readonly IUserDelegationConfiguration _userDelegationConfiguration;

    public UserDelegationAppService(
        IRepository<UserDelegation, Guid> userDelegationRepository,
        IRepository<User, Guid> userRepository,
        IUserDelegationManager userDelegationManager,
        IUserDelegationConfiguration userDelegationConfiguration)
    {
        _userDelegationRepository = userDelegationRepository;
        _userRepository = userRepository;
        _userDelegationManager = userDelegationManager;
        _userDelegationConfiguration = userDelegationConfiguration;
    }

    public async Task<PagedResultDto<UserDelegationDto>> GetDelegatedUsers(GetUserDelegationsInput input)
    {
        CheckUserDelegationOperation();

        var query = CreateDelegatedUsersQuery(sourceUserId: ADTOSharpSession.GetUserId(), targetUserId: null, input.Sorting);
        var totalCount = await query.CountAsync();
        if (input.PageNumber <= 1)
            input.PageNumber = 1;

        var userDelegations = await query
            .Skip((input.PageNumber - 1) * input.PageSize)
            .Take(input.PageSize)
            .ToListAsync();

        return new PagedResultDto<UserDelegationDto>(
            totalCount,
            userDelegations
        );
    }
    /// <summary>
    /// 根据流程指定时间获取流程委托人
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<List<UserDelegationDto>> GetWorkFlowDelegatedUsers(Guid userId, DateTime dateTime)
    {
        CheckUserDelegationOperation();
        var query = CreateDelegatedUsersQuery(userId, targetUserId: null, "userName DESC", dateTime);
        var userDelegations = await query.ToListAsync();
        return userDelegations;
    }
    public async Task DelegateNewUser(CreateUserDelegationDto input)
    {
        if (input.TargetUserId == ADTOSharpSession.GetUserId())
        {
            throw new UserFriendlyException(L("SelfUserDelegationErrorMessage"));
        }

        CheckUserDelegationOperation();

        var delegation = ObjectMapper.Map<UserDelegation>(input);

        delegation.TenantId = ADTOSharpSession.TenantId;
        delegation.SourceUserId = ADTOSharpSession.GetUserId();

        await _userDelegationRepository.InsertAsync(delegation);
    }

    public async Task RemoveDelegation(EntityDto<Guid> input)
    {
        CheckUserDelegationOperation();

        await _userDelegationManager.RemoveDelegationAsync(input.Id, ADTOSharpSession.ToUserIdentifier());
    }

    /// <summary>
    /// Returns active user delegations for current user
    /// </summary>
    /// <returns></returns>
    public async Task<List<UserDelegationDto>> GetActiveUserDelegations()
    {
        var query = CreateActiveUserDelegationsQuery(ADTOSharpSession.GetUserId(), "username");
        query = query.Where(e => e.EndTime >= Clock.Now);
        return await query.ToListAsync();
    }

    private void CheckUserDelegationOperation()
    {
        if (ADTOSharpSession.ImpersonatorUserId.HasValue)
        {
            throw new Exception("Cannot execute user delegation operations with an impersonated user !");
        }

        if (!_userDelegationConfiguration.IsEnabled)
        {
            throw new Exception("User delegation configuration is not enabled !");
        }
    }

    private IQueryable<UserDelegationDto> CreateDelegatedUsersQuery(Guid? sourceUserId, Guid? targetUserId, string sorting, DateTime? nowDate = null)
    {
        var query = _userDelegationRepository.GetAll()
            .WhereIf(sourceUserId.HasValue, e => e.SourceUserId == sourceUserId)
            .WhereIf(targetUserId.HasValue, e => e.TargetUserId == targetUserId)
            .WhereIf(nowDate.HasValue, e => nowDate >= e.StartTime && nowDate <= e.EndTime);

        return (from userDelegation in query
                join targetUser in _userRepository.GetAll() on userDelegation.TargetUserId equals targetUser.Id into targetUserJoined
                from targetUser in targetUserJoined.DefaultIfEmpty()
                select new UserDelegationDto
                {
                    Id = userDelegation.Id == Guid.Empty ? Guid.NewGuid() : userDelegation.Id,
                    UserId = targetUser.Id == Guid.Empty ? targetUser.Id : Guid.NewGuid(),
                    Username = targetUser.UserName,
                    Name = targetUser.Name,
                    DepartmentId = targetUser.DepartmentId ?? Guid.Empty,
                    CompanyId = targetUser.CompanyId ?? Guid.Empty,
                    StartTime = userDelegation.StartTime,
                    EndTime = userDelegation.EndTime
                }).OrderBy(sorting);
    }

    private IQueryable<UserDelegationDto> CreateActiveUserDelegationsQuery(Guid targetUserId, string sorting)
    {
        var query = _userDelegationRepository.GetAll()
            .Where(e => e.TargetUserId == targetUserId);

        return (from userDelegation in query
                join sourceUser in _userRepository.GetAll() on userDelegation.SourceUserId equals sourceUser.Id into sourceUserJoined
                from sourceUser in sourceUserJoined.DefaultIfEmpty()
                select new UserDelegationDto
                {
                    Id = userDelegation.Id,
                    UserId = sourceUser.Id,
                    Username = sourceUser.UserName,
                    Name = sourceUser.Name,
                    DepartmentId = sourceUser.DepartmentId??Guid.Empty,
                    CompanyId = sourceUser.CompanyId ?? Guid.Empty,
                    StartTime = userDelegation.StartTime,
                    EndTime = userDelegation.EndTime
                }).OrderBy(sorting);
    }
}

