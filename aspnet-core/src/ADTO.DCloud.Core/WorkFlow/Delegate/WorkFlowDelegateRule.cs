

using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Delegate
{
    /// <summary>
    /// 工作流委托规则
    /// </summary>
    [Table("WorkFlowDelegaterules")]
    public class WorkFlowDelegaterule:Entity<Guid>, ICreationAudited, IPassivable,IRemark  , IMayHaveTenant
    {
        #region 实体成员 
        /// <summary> 
        /// 被委托人Id 
        /// </summary> 
        public Guid ToUserId { get; set; }
        /// <summary> 
        /// 被委托人名称 
        /// </summary> 
        [StringLength(50)]
        public string ToUserName { get; set; }
        /// <summary> 
        /// 委托开始时间 
        /// </summary> 
        public DateTime? BeginDate { get; set; }
        /// <summary> 
        /// 委托结束时间 
        /// </summary> 
        public DateTime? EndDate { get; set; }
     

        /// <summary> 
        /// 委托类型 1 待发委托 0或其他 审批委托
        /// </summary> 
        public int? Type { get; set; }
        /// <summary> 
        /// 有效标志1有效 0 无效 
        /// </summary> 
        public bool IsActive {  get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 多租户
        /// </summary>
        public Guid? TenantId {  get; set; }
        public Guid? CreatorUserId {  get; set; }
        public DateTime CreationTime {  get; set; }
        #endregion
    }
}
