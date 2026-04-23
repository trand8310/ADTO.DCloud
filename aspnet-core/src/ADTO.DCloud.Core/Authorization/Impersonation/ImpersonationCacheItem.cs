using System;

namespace ADTO.DCloud.Authorization.Impersonation
{
    [Serializable]
    public class ImpersonationCacheItem
    {
        public const string CacheName = "AppImpersonationCache";

        /// <summary>
        /// 模拟租户Id
        /// </summary>
        public Guid? ImpersonatorTenantId { get; set; }
        /// <summary>
        /// 模拟用户Id
        /// </summary>

        public Guid ImpersonatorUserId { get; set; }
        /// <summary>
        /// 目标租户Id
        /// </summary>
        public Guid? TargetTenantId { get; set; }
        /// <summary>
        /// 目标用户Id
        /// </summary>
        public Guid TargetUserId { get; set; }
        /// <summary>
        /// 是否退回模拟帐号
        /// </summary>
        public bool IsBackToImpersonator { get; set; }

        public ImpersonationCacheItem()
        {
            
        }

        public ImpersonationCacheItem(Guid? targetTenantId, Guid targetUserId, bool isBackToImpersonator)
        {
            TargetTenantId = targetTenantId;
            TargetUserId = targetUserId;
            IsBackToImpersonator = isBackToImpersonator;
        }
    }
}