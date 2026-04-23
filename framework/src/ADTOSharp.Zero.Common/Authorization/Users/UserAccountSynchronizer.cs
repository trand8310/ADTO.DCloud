using ADTOSharp.Dependency;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Domain.Uow;
using ADTOSharp.Events.Bus.Entities;
using ADTOSharp.Events.Bus.Handlers;
using ADTOSharp.MultiTenancy;
using System;

namespace ADTOSharp.Authorization.Users
{
    /// <summary>
    /// Synchronizes a user's information to user account.
    /// </summary>
    public class UserAccountSynchronizer :
        IEventHandler<EntityCreatedEventData<ADTOSharpUserBase>>,
        IEventHandler<EntityDeletedEventData<ADTOSharpUserBase>>,
        IEventHandler<EntityUpdatedEventData<ADTOSharpUserBase>>,
        IEventHandler<EntityDeletedEventData<ADTOSharpTenantBase>>,
        ITransientDependency
    {
        private readonly IRepository<UserAccount, Guid> _userAccountRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        /// <summary>
        /// Constructor
        /// </summary>
        public UserAccountSynchronizer(
            IRepository<UserAccount, Guid> userAccountRepository,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _userAccountRepository = userAccountRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        /// <summary>
        /// Handles creation event of user
        /// </summary>
        public virtual void HandleEvent(EntityCreatedEventData<ADTOSharpUserBase> eventData)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(null))
                {
                    var userAccount = _userAccountRepository.FirstOrDefault(
                        ua => ua.TenantId == eventData.Entity.TenantId && ua.UserId == eventData.Entity.Id
                    );

                    if (userAccount == null)
                    {
                        _userAccountRepository.Insert(new UserAccount
                        {
                            TenantId = eventData.Entity.TenantId,
                            UserName = eventData.Entity.UserName,
                            UserId = eventData.Entity.Id,
                            EmailAddress = eventData.Entity.EmailAddress
                        });
                    }
                    else
                    {
                        userAccount.UserName = eventData.Entity.UserName;
                        userAccount.EmailAddress = eventData.Entity.EmailAddress;
                        _userAccountRepository.Update(userAccount);
                    }
                }
            });
        }

        /// <summary>
        /// Handles deletion event of user
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void HandleEvent(EntityDeletedEventData<ADTOSharpUserBase> eventData)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(null))
                {
                    var userAccount = _userAccountRepository.FirstOrDefault(
                        ua => ua.TenantId == eventData.Entity.TenantId && ua.UserId == eventData.Entity.Id
                    );
                    
                    if (userAccount != null)
                    {
                        _userAccountRepository.Delete(userAccount);
                    }
                }
            });
        }

        /// <summary>
        /// Handles update event of user
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void HandleEvent(EntityUpdatedEventData<ADTOSharpUserBase> eventData)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(null))
                {
                    var userAccount = _userAccountRepository.FirstOrDefault(ua =>
                        ua.TenantId == eventData.Entity.TenantId && ua.UserId == eventData.Entity.Id
                    );
                    
                    if (userAccount != null)
                    {
                        userAccount.UserName = eventData.Entity.UserName;
                        userAccount.EmailAddress = eventData.Entity.EmailAddress;
                        _userAccountRepository.Update(userAccount);
                    }
                }
            });
        }

        /// <summary>
        /// Handles deletion event of tenant
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void HandleEvent(EntityDeletedEventData<ADTOSharpTenantBase> eventData)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetTenantId(null))
                {
                    _userAccountRepository.Delete(ua => ua.TenantId == eventData.Entity.Id);
                }
            });
        }
    }
}
