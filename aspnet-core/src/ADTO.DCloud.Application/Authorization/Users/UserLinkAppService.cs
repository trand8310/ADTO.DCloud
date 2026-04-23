using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Auditing;
using ADTOSharp.Authorization;
using ADTOSharp.Authorization.Users;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Runtime.Session;
using ADTOSharp.UI;
using Microsoft.EntityFrameworkCore;
using ADTO.DCloud.Authorization.Users.Dto;
using ADTO.DCloud.MultiTenancy;

namespace ADTO.DCloud.Authorization.Users;

/// <summary>
/// 用户绑定服务,用于将第三方登录,绑定到系统用户
/// </summary>
[ADTOSharpAuthorize]
public class UserLinkAppService : DCloudAppServiceBase, IUserLinkAppService
{
    private readonly ADTOSharpLoginResultTypeHelper _adtosharpLoginResultTypeHelper;
    private readonly IUserLinkManager _userLinkManager;
    private readonly IRepository<Tenant, Guid> _tenantRepository;
    private readonly IRepository<UserAccount, Guid> _userAccountRepository;
    private readonly LogInManager _logInManager;

    public UserLinkAppService(
        ADTOSharpLoginResultTypeHelper adtosharpLoginResultTypeHelper,
        IUserLinkManager userLinkManager,
        IRepository<Tenant, Guid> tenantRepository,
        IRepository<UserAccount, Guid> userAccountRepository,
        LogInManager logInManager)
    {
        _adtosharpLoginResultTypeHelper = adtosharpLoginResultTypeHelper;
        _userLinkManager = userLinkManager;
        _tenantRepository = tenantRepository;
        _userAccountRepository = userAccountRepository;
        _logInManager = logInManager;
    }

    /// <summary>
    /// 关联用户
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    public async Task LinkToUser(LinkToUserInput input)
    {
        var loginResult = await _logInManager.LoginAsync(input.Username, input.Password, input.TenancyName);

        if (loginResult.Result != ADTOSharpLoginResultType.Success)
        {
            throw _adtosharpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(loginResult.Result, input.Username, input.TenancyName);
        }

        if (ADTOSharpSession.IsUser(loginResult.User))
        {
            throw new UserFriendlyException(L("YouCannotLinkToSameAccount"));
        }

        if (loginResult.User.ShouldChangePasswordOnNextLogin)
        {
            throw new UserFriendlyException(L("ChangePasswordBeforeLinkToAnAccount"));
        }

        var currentUser = await GetCurrentUserAsync();
        await _userLinkManager.Link(currentUser, loginResult.User);
    }

    /// <summary>
    /// 获取当前用户的所有关联帐号
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<PagedResultDto<LinkedUserDto>> GetLinkedUsers(GetLinkedUsersInput input)
    {
        var currentUserAccount = await _userLinkManager.GetUserAccountAsync(ADTOSharpSession.ToUserIdentifier());
        if (currentUserAccount == null)
        {
            return new PagedResultDto<LinkedUserDto>(0, new List<LinkedUserDto>());
        }

        var query = CreateLinkedUsersQuery(currentUserAccount, input.Sorting);

        var totalCount = await query.CountAsync();
        var skipCount = ((input.PageNumber < 1 ? 1 : input.PageNumber) - 1) * input.PageSize;


        var linkedUsers = await query
            .Skip(skipCount)
            .Take(input.PageSize)
            .ToListAsync();

        return new PagedResultDto<LinkedUserDto>(
            totalCount,
            linkedUsers
        );
    }

    /// <summary>
    /// 获取当前用户最近绑定的3个帐号
    /// </summary>
    /// <returns></returns>
    [DisableAuditing]
    public async Task<ListResultDto<LinkedUserDto>> GetRecentlyUsedLinkedUsers()
    {
        var currentUserAccount = await _userLinkManager.GetUserAccountAsync(ADTOSharpSession.ToUserIdentifier());
        if (currentUserAccount == null)
        {
            return new ListResultDto<LinkedUserDto>();
        }

        var query = CreateLinkedUsersQuery(currentUserAccount, "TenancyName, Username");
        var recentlyUsedlinkedUsers = await query.Take(3).ToListAsync();

        return new ListResultDto<LinkedUserDto>(recentlyUsedlinkedUsers);
    }
    /// <summary>
    /// 解除绑定
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task UnlinkUser(UnlinkUserInput input)
    {
        var currentUserAccount = await _userLinkManager.GetUserAccountAsync(ADTOSharpSession.ToUserIdentifier());

        if (!currentUserAccount.UserLinkId.HasValue)
        {
            throw new Exception(L("YouAreNotLinkedToAnyAccount"));
        }

        if (!await _userLinkManager.AreUsersLinked(ADTOSharpSession.ToUserIdentifier(), input.ToUserIdentifier()))
        {
            return;
        }

        await _userLinkManager.Unlink(input.ToUserIdentifier());
    }

    private IQueryable<LinkedUserDto> CreateLinkedUsersQuery(UserAccount currentUserAccount, string sorting)
    {
        var currentUserIdentifier = ADTOSharpSession.ToUserIdentifier();

        return (from userAccount in _userAccountRepository.GetAll()
                join tenant in _tenantRepository.GetAll() on userAccount.TenantId equals tenant.Id into tenantJoined
                from tenant in tenantJoined.DefaultIfEmpty()
                where
                    (userAccount.TenantId != currentUserIdentifier.TenantId ||
                    userAccount.UserId != currentUserIdentifier.UserId) &&
                    userAccount.UserLinkId.HasValue &&
                    userAccount.UserLinkId == currentUserAccount.UserLinkId
                select new LinkedUserDto
                {
                    Id = userAccount.UserId,
                    TenantId = userAccount.TenantId,
                    TenancyName = tenant == null ? "." : tenant.TenancyName,
                    Username = userAccount.UserName
                }).OrderBy(sorting);
    }
}

