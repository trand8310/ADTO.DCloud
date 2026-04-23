using System;
using System.Threading.Tasks;
using ADTOSharp.Configuration;
using ADTOSharp.Dependency;
using ADTOSharp.Zero.Configuration;

namespace ADTOSharp.Organizations
{
    /// <summary>
    /// Implements <see cref="IOrganizationUnitSettings"/> to get settings from <see cref="ISettingManager"/>.
    /// </summary>
    public class OrganizationUnitSettings : IOrganizationUnitSettings, ITransientDependency
    {
        /// <summary>
        /// Maximum allowed organization unit membership count for a user.
        /// Returns value for current tenant.
        /// </summary>
        public int MaxUserMembershipCount
        {
            get
            {
                return _settingManager.GetSettingValue<int>(ADTOSharpZeroSettingNames.OrganizationUnits.MaxUserMembershipCount);
            }
        }

        private readonly ISettingManager _settingManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrganizationUnitSettings"/> class.
        /// </summary>
        public OrganizationUnitSettings(ISettingManager settingManager)
        {
            _settingManager = settingManager;
        }

        /// <summary>
        /// Maximum allowed organization unit membership count for a user.
        /// Returns value for given tenant.
        /// </summary>
        public async Task<int> GetMaxUserMembershipCountAsync(Guid? tenantId)
        {
            if (tenantId.HasValue)
            {
                return await _settingManager.GetSettingValueForTenantAsync<int>(ADTOSharpZeroSettingNames.OrganizationUnits.MaxUserMembershipCount, tenantId.Value);
            }
            else
            {
                return await _settingManager.GetSettingValueForApplicationAsync<int>(ADTOSharpZeroSettingNames.OrganizationUnits.MaxUserMembershipCount);
            }
        }

        /// <summary>
        /// Maximum allowed organization unit membership count for a user.
        /// Returns value for given tenant.
        /// </summary>
        public int GetMaxUserMembershipCount(Guid? tenantId)
        {
            if (tenantId.HasValue)
            {
                return _settingManager.GetSettingValueForTenant<int>(ADTOSharpZeroSettingNames.OrganizationUnits.MaxUserMembershipCount, tenantId.Value);
            }
            else
            {
                return _settingManager.GetSettingValueForApplication<int>(ADTOSharpZeroSettingNames.OrganizationUnits.MaxUserMembershipCount);
            }
        }

        public async Task SetMaxUserMembershipCountAsync(Guid? tenantId, int value)
        {
            if (tenantId.HasValue)
            {
                await _settingManager.ChangeSettingForTenantAsync(tenantId.Value, ADTOSharpZeroSettingNames.OrganizationUnits.MaxUserMembershipCount, value.ToString());
            }
            else
            {
                await _settingManager.ChangeSettingForApplicationAsync(ADTOSharpZeroSettingNames.OrganizationUnits.MaxUserMembershipCount, value.ToString());
            }
        }

        public void SetMaxUserMembershipCount(Guid? tenantId, int value)
        {
            if (tenantId.HasValue)
            {
                _settingManager.ChangeSettingForTenant(tenantId.Value, ADTOSharpZeroSettingNames.OrganizationUnits.MaxUserMembershipCount, value.ToString());
            }
            else
            {
                _settingManager.ChangeSettingForApplication(ADTOSharpZeroSettingNames.OrganizationUnits.MaxUserMembershipCount, value.ToString());
            }
        }
    }
}