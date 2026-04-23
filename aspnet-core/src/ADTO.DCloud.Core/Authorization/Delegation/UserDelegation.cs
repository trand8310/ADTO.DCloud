using System;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using ADTOSharp.Timing;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace ADTO.DCloud.Authorization.Delegation
{
    /// <summary>
    /// 用户委托
    /// </summary>
    [Table("UserDelegations"), Description("用户委托")]
    public class UserDelegation : FullAuditedEntity<Guid>, IMayHaveTenant
    {
        /// <summary>
        /// 被委托人
        /// </summary>
        [Description("被委托人")]
        public Guid SourceUserId { get; set; }

        /// <summary>
        /// 委托人 <see cref="SourceUserId"/>
        /// </summary>
        [Description("委托人")]
        public Guid TargetUserId { get; set; }

        /// <summary>
        /// 租户,委托与被委托,需要存在相同的租户范围内
        /// </summary>
        [Description("租户")]
        public Guid? TenantId { get; set; }

        /// <summary>
        /// 委托开始时间
        /// </summary>
        [Description("开始时间")]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 委托结束时间
        /// </summary>
        [Description("结束时间")]
        public DateTime EndTime { get; set; }

        public bool IsCreatedByUser(Guid userId){
            return SourceUserId == userId;
        }

        public bool IsExpired(){
            return EndTime <= Clock.Now;
        }

        public bool IsValid(){
            return StartTime <= Clock.Now && !IsExpired();
        }
    }
}
