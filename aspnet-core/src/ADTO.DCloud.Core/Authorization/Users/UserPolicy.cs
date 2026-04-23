using System;
using System.Threading.Tasks;
using ADTO.DCloud.Features;
using ADTOSharp.Application.Features;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Extensions;
using ADTOSharp.UI;


namespace ADTO.DCloud.Authorization.Users;

public class UserPolicy : DCloudServiceBase, IUserPolicy
{
    private readonly IFeatureChecker _featureChecker;
    private readonly IRepository<User, Guid> _userRepository;

    public UserPolicy(IFeatureChecker featureChecker,IRepository<User, Guid> userRepository)
    {
        _featureChecker = featureChecker;
        _userRepository = userRepository;
    }

    public async Task CheckMaxUserCountAsync(Guid tenantId)
    {
        var maxUserCount = (await _featureChecker.GetValueAsync(tenantId, AppFeatures.MaxUserCount)).To<int>();
        if (maxUserCount <= 0)
        {
            return;
        }

        var currentUserCount = await _userRepository.CountAsync();
        if (currentUserCount >= maxUserCount)
        {
            throw new UserFriendlyException(L("MaximumUserCount_Error_Message"), L("MaximumUserCount_Error_Detail", maxUserCount));
        }
    }
}
