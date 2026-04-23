using ADTOSharp.Authorization.Users;
using ADTOSharp.Domain.Repositories;
using ADTOSharp.Domain.Uow;
using ADTOSharp.Linq;
using ADTOSharp.Organizations;
using ADTO.DCloud.Authorization.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using ADTO.DCloud.Organizations;
using ADTOSharp;
using System.Timers;

namespace ADTO.DCloud.Authorization.Users
{
    public class UserStore : ADTOSharpUserStore<Role, User>
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<Role, Guid> _roleRepository;
        private readonly IRepository<UserRole, Guid> _userRoleRepository;
        private readonly IRepository<OrganizationUnitRole, Guid> _organizationUnitRoleRepository;

        private readonly IRepository<UserOrganizationUnit, Guid> _userOrganizationUnitRepository;
        public UserStore(
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<User, Guid> userRepository,
            IRepository<Role, Guid> roleRepository,
            IRepository<UserRole, Guid> userRoleRepository,
            IRepository<UserLogin, Guid> userLoginRepository,
            IRepository<UserClaim, Guid> userClaimRepository,
            IRepository<UserPermissionSetting, Guid> userPermissionSettingRepository,
            IRepository<UserOrganizationUnit, Guid> userOrganizationUnitRepository,
            IRepository<OrganizationUnitRole, Guid> organizationUnitRoleRepository,
            IRepository<UserToken, Guid> userTokenRepository
        ) 
            : base(unitOfWorkManager,
                  userRepository,
                  roleRepository,
                  userRoleRepository,
                  userLoginRepository,
                  userClaimRepository,
                  userPermissionSettingRepository,
                  userOrganizationUnitRepository,
                  organizationUnitRoleRepository,
                  userTokenRepository
            )
        {
            _unitOfWorkManager = unitOfWorkManager;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _organizationUnitRoleRepository = organizationUnitRoleRepository;
            _userOrganizationUnitRepository = userOrganizationUnitRepository;
        }


        public async Task ReplaceCodesAsync(User user, IEnumerable<string> recoveryCodes, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var mergedCodes = string.Join(";", recoveryCodes);
            user.RecoveryCode = mergedCodes;
            await UpdateAsync(user, cancellationToken);
        }

        public async Task<bool> RedeemCodeAsync(User user, string code, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (code == null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            var mergedCodes = user.RecoveryCode ?? "";
            var splitCodes = mergedCodes.Split(';');

            if (!splitCodes.Contains(code))
            {
                return false;
            }

            var updatedCodes = new List<string>(splitCodes.Where(s => s != code));
            await ReplaceCodesAsync(user, updatedCodes, cancellationToken).ConfigureAwait(false);
            return true;
        }

        public Task<int> CountCodesAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var mergedCodes = user.RecoveryCode ?? "";

            return Task.FromResult(mergedCodes.Length > 0 ? mergedCodes.Split(';').Length : 0);
        }


        /// <summary>
        /// 获取用户所有的角色,含直接赋予的+部门赋予的
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public  async Task<IList<Role>> GetUserRolesAsync(User user, CancellationToken cancellationToken = default(CancellationToken))
        {

            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                Check.NotNull(user, nameof(user));

                var userRoles = await AsyncQueryableExecuter.ToListAsync(from userRole in await _userRoleRepository.GetAllAsync()
                                                                         join role in await _roleRepository.GetAllAsync() on userRole.RoleId equals role.Id
                                                                         where userRole.UserId == user.Id
                                                                         select role);

                var userOrganizationUnitRoles = await AsyncQueryableExecuter.ToListAsync(
                    from userOu in await _userOrganizationUnitRepository.GetAllAsync()
                    join roleOu in await _organizationUnitRoleRepository.GetAllAsync() on userOu.OrganizationUnitId equals roleOu
                        .OrganizationUnitId
                    join userOuRoles in await _roleRepository.GetAllAsync() on roleOu.RoleId equals userOuRoles.Id
                    where userOu.UserId == user.Id
                    select userOuRoles);

                return userRoles.Union(userOrganizationUnitRoles).ToList();
            });
        }

    }
}
