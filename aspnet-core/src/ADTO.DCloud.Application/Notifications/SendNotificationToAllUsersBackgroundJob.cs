using System;
using System.Linq;
using System.Threading.Tasks;
using ADTOSharp;
using ADTOSharp.Authorization.Users;
using ADTOSharp.BackgroundJobs;
using ADTOSharp.Dependency;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Domain.Uow;
using Microsoft.EntityFrameworkCore;

namespace ADTO.DCloud.Notifications
{
    public class SendNotificationToAllUsersBackgroundJob : AsyncBackgroundJob<SendNotificationToAllUsersArgs>,
        ITransientDependency
    {
        private const int MaxUserCount = 1000;

        private readonly IRepository<UserAccount, Guid> _userAccountRepository;
        private readonly IAppNotifier _appNotifier;

        public SendNotificationToAllUsersBackgroundJob(IRepository<UserAccount, Guid> userAccountRepository,
            IAppNotifier appNotifier)
        {
            _userAccountRepository = userAccountRepository;
            _appNotifier = appNotifier;
        }

        public override async Task ExecuteAsync(SendNotificationToAllUsersArgs toAllUsersArgs)
        {
            var userCount = await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
                await _userAccountRepository.GetAll().LongCountAsync()
            );

            if (userCount == 0)
            {
                return;
            }

            var loopCount = userCount / MaxUserCount + 1;

            for (var i = 0; i < loopCount; i++)
            {
                var userIds = await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
                {
                    return await _userAccountRepository.GetAll().Skip(i * MaxUserCount)
                        .AsNoTracking()
                        .Take(MaxUserCount)
                        .Select(u => new UserIdentifier(u.TenantId, u.UserId))
                        .ToArrayAsync();
                });

                await _appNotifier.SendMessageAsync(
                    toAllUsersArgs.NotificationName,
                    toAllUsersArgs.Message,
                    userIds,
                    toAllUsersArgs.Severity
                );
            }
        }
    }
}