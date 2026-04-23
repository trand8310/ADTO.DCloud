using ADTO.DCloud.Authorization.Users;
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
    /// 描 述：工作流模板信息(新)
    /// </summary>
    [Description("工作流模板信息"), Table("WorkFlowSchemeinfos")]
    public class WorkFlowSchemeinfo : FullAuditedAggregateRoot<Guid>, IRemark, IPassivable, IMayHaveTenant, IDisplayOrder
    {
        #region 实体成员
        [ForeignKey("CreatorUserId")]
        public virtual User? CreatorUser { get; set; }


        /// <summary> 
        /// 流程编码 
        /// </summary> 
        [StringLength(50), Description("流程编码")]
        public string Code { get; set; }
        /// <summary> 
        /// 流程模板名称 
        /// </summary> 
        [StringLength(200), Description("流程模板名称")]
        public string Name { get; set; }
        /// <summary> 
        /// 流程分类 
        /// </summary> 
        [StringLength(50), Description("流程分类")]
        public string Category { get; set; }

        /// <summary>
        /// 模板颜色
        /// </summary>
        [StringLength(50), Description("模板颜色")]
        public string Color { get; set; }
        /// <summary>
        /// 模板图标
        /// </summary>
        [StringLength(50), Description("模板图标")]
        public string Icon { get; set; }

        /// <summary>
        /// 模板图标地址
        /// </summary>
        [StringLength(225), Description("模板图标")]
        public string IconUrl { get; set; }
        /// <summary> 
        /// 流程模板ID 
        /// </summary> 
        [Description("流程模板ID")]
        public Guid SchemeId { get; set; }
        /// <summary> 
        /// 是否有效 
        /// </summary> 
        [Description("是否有效")]
        public bool IsActive { get; set; }
        /// <summary> 
        /// 是否在我的任务允许发起 1允许 2不允许 
        /// </summary> 
        [Description("是否在我的任务允许发起 1允许 2不允许 ")]
        public int? Mark { get; set; }
        /// <summary> 
        /// 是否在App上允许发起 1允许 2不允许 
        /// </summary> 
        [Description("是否在App上允许发起 1允许 2不允许")]
        public int? IsInApp { get; set; }
        /// <summary>
        /// 流程权限类型1.都能发起 2.指定用户可发起
        /// </summary>
        [Description("流程权限类型1.都能发起 2.指定用户可发起")]
        public int? AuthType { get; set; }
        /// <summary>
        /// 流程监控权限类型1.都有监控权限 2.指定用户可发起
        /// </summary>
        [Description("流程监控权限类型1.都有监控权限 2.指定用户可发起")]
        public int? MonitorAuthType { get; set; }

        /// <summary>
        /// 密级(0公开，1内部，2秘密，3机密，4绝密)
        /// </summary>
        [Description("密级(0公开，1内部，2秘密，3机密，4绝密)")]
        public int? SecretLevel { get; set; }

        /// <summary> 
        /// 归属单位 
        /// </summary> 
        [Description("归属单位")]
        public Guid? CompanyId { get; set; }

        /// <summary>
        /// 1.外部流程 2.系统本身流程
        /// </summary>
        [Description("1.外部流程 2.系统本身流程")]
        public int? IsOther { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Description("备注")]
        [StringLength(200)]
        public string Remark { get; set; }

        #endregion
        #region 多租户

        /// <summary>
        /// 排序
        /// </summary>
        public int DisplayOrder { get; set; }
        /// <summary>
        /// 多租户
        /// </summary>
        public Guid? TenantId { get; set; }
        #endregion
    }
}
