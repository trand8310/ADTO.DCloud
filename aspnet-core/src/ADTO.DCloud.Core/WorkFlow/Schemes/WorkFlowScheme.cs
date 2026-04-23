using ADTO.DCloud.Authorization.Users;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Schemes
{
    /// <summary>
    /// 日期：2024-12-26
    /// 工作流模板
    /// </summary>
    [Description("工作流模板"),Table("WorkFlowSchemes")]
    public class WorkFlowScheme : Entity<Guid>, ICreationAudited, IMayHaveTenant
    {
        #region 实体成员 


        /// <summary> 
        /// 流程模板信息主键 
        /// </summary> 
        [Description("流程模板信息主键")]
        public Guid SchemeInfoId { get; set; }
        /// <summary> 
        /// 1.正式2.草稿 
        /// </summary> 
        [Description("1.正式2.草稿 3.临时修改模版")]
        public int? Type { get; set; }
        /// <summary> 
        /// 流程模板内容 
        /// </summary> 
        [Description("流程模板内容"), StringLength(int.MaxValue)]
        public string Content { get; set; }
        #endregion
        #region 多租户

        /// <summary>
        /// 多租户
        /// </summary>
        public Guid? TenantId { get; set; }

        [ForeignKey("CreatorUserId")]
        public virtual User? CreatorUser { get; set; }
        public Guid? CreatorUserId {  get; set; }

        public DateTime CreationTime {  get; set; }
        #endregion
    }
}
