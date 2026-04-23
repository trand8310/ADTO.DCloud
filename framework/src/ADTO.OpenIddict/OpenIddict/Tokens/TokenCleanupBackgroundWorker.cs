using ADTOSharp.Dependency;
using ADTOSharp.Domain.Uow;
using ADTOSharp.Logging;
using ADTOSharp.Threading.BackgroundWorkers;
using ADTOSharp.Threading.Timers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;


namespace ADTO.OpenIddict.Tokens;

public class TokenCleanupBackgroundWorker : AsyncPeriodicBackgroundWorkerBase, ISingletonDependency
{
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly TokenCleanupService _tokenCleanupService;
    public TokenCleanupBackgroundWorker(
        ADTOSharpAsyncTimer timer,
        IServiceScopeFactory serviceScopeFactory,
        IOptionsMonitor<TokenCleanupOptions> cleanupOptions,
        IUnitOfWorkManager unitOfWorkManager,
        TokenCleanupService tokenCleanupService)
        : base(timer)
    {
      
        timer.Period = cleanupOptions.CurrentValue.CleanupPeriod;
        _unitOfWorkManager = unitOfWorkManager;
        _tokenCleanupService = tokenCleanupService;

    }
    protected override async Task DoWorkAsync()
    {
        await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
        {
            await _tokenCleanupService.CleanAsync();
            Logger.Info($"Lock is released for {nameof(TokenCleanupBackgroundWorker)}");
        });
    }
}
