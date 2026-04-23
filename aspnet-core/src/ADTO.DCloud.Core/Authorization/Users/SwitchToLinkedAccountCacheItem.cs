using System;

namespace ADTO.DCloud.Authorization.Users
{
    [Serializable]
    public class SwitchToLinkedAccountCacheItem
    {
        public const string CacheName = "AppSwitchToLinkedAccountCache";

        public Guid? TargetTenantId { get; set; }

        public Guid TargetUserId { get; set; }

        public Guid? ImpersonatorTenantId { get; set; }

        public Guid? ImpersonatorUserId { get; set; }

        public SwitchToLinkedAccountCacheItem()
        {

        }

        public SwitchToLinkedAccountCacheItem(
            Guid? targetTenantId,
            Guid targetUserId,
            Guid? impersonatorTenantId,
            Guid? impersonatorUserId
            )
        {
            TargetTenantId = targetTenantId;
            TargetUserId = targetUserId;
            ImpersonatorTenantId = impersonatorTenantId;
            ImpersonatorUserId = impersonatorUserId;
        }
    }
}
