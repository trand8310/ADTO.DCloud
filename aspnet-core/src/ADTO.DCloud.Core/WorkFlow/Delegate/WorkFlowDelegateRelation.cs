using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Delegate
{
    /// <summary>
    /// 工作流委托模板关系表
    /// </summary>
    [Table("WorkFlowDelegateRelations")]
    public class WorkFlowDelegateRelation:Entity<Guid>,IMayHaveTenant
    {
        #region 实体成员
        /// <summary>
        /// 委托规则主键 
        /// </summary> 
        public Guid DelegateRuleId { get; set; }
        /// <summary> 
        /// 流程模板信息主键 
        /// </summary> 
        public string SchemeInfoCode { get; set; }
        /// <summary>
        /// 多租户
        /// </summary>
        public Guid? TenantId { get; set; }
        #endregion
    }
}
