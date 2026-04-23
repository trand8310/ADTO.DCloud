using ADTOSharp.Dependency;
using ADTOSharp.Domain.Uow;
using ADTOSharp.Threading.BackgroundWorkers;
using ADTOSharp.Threading.Timers;

namespace ADTO.DCloud.Authorization.Users.Password;

/// <summary>
/// 密码过期标记后台服务
/// </summary>
public class PasswordExpirationBackgroundWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
{
    private const int CheckPeriodAsMilliseconds = 1 * 60 * 60 * 1000 * 24; //1 day

    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly IPasswordExpirationService _passwordExpirationService;

    public PasswordExpirationBackgroundWorker(
        ADTOSharpTimer timer,
        IUnitOfWorkManager unitOfWorkManager,
        IPasswordExpirationService passwordExpirationService)
        : base(timer)
    {
        _unitOfWorkManager = unitOfWorkManager;
        _passwordExpirationService = passwordExpirationService;

        Timer.Period = CheckPeriodAsMilliseconds;
        Timer.RunOnStart = true;

        LocalizationSourceName = DCloudConsts.LocalizationSourceName;
    }

    protected override void DoWork()
    {
        _unitOfWorkManager.WithUnitOfWork(() =>
        {
            _passwordExpirationService.ForcePasswordExpiredUsersToChangeTheirPassword();
        });
    }
}

