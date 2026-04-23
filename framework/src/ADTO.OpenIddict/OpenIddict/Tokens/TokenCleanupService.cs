using ADTOSharp.Dependency;
using ADTOSharp.Logging;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using System;
using System.Threading.Tasks;
 

namespace ADTO.OpenIddict.Tokens;

/// <summary>
/// 注意：此后台任务负责自动删除无用的令牌/授权（即不再有效的令牌以及没有有效令牌关联的临时授权）。
/// 说明：由于与临时授权相关的令牌并非作为同一操作的一部分被删除，
/// 因此在删除不再有任何令牌的临时授权之前，必须先删除这些令牌。
/// </summary>

public class TokenCleanupService : ITransientDependency
{
    public ILogger<TokenCleanupService> Logger { get; set; }
    protected TokenCleanupOptions CleanupOptions { get; }
    protected IOpenIddictTokenManager TokenManager { get; }
    protected IOpenIddictAuthorizationManager AuthorizationManager { get; }

    public TokenCleanupService(
        IOptionsMonitor<TokenCleanupOptions> cleanupOptions,
        IOpenIddictTokenManager tokenManager,
        IOpenIddictAuthorizationManager authorizationManager)
    {
        Logger = NullLogger<TokenCleanupService>.Instance;;

        CleanupOptions = cleanupOptions.CurrentValue;
        TokenManager = tokenManager;
        AuthorizationManager = authorizationManager;
    }

    public virtual async Task CleanAsync()
    {
        Logger.LogInformation("Start cleanup.");

        if (!CleanupOptions.DisableTokenPruning)
        {
            Logger.LogInformation("Start cleanup tokens.");

            var threshold = DateTimeOffset.UtcNow - CleanupOptions.MinimumTokenLifespan;
            try
            {
                await TokenManager.PruneAsync(threshold);
            }
            catch (Exception exception)
            {
                LogHelper.LogException(exception);
            }
        }

        if (!CleanupOptions.DisableAuthorizationPruning)
        {
            Logger.LogInformation("Start cleanup authorizations.");

            var threshold = DateTimeOffset.UtcNow - CleanupOptions.MinimumAuthorizationLifespan;
            try
            {
                await AuthorizationManager.PruneAsync(threshold);
            }
            catch (Exception exception)
            {
                LogHelper.LogException(exception);
            }
        }
    }
}
