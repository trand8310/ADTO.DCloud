using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using ADTOSharp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Schemes.Dto
{
    /// <summary>
    /// 
    /// </summary>
    [AutoMapTo(typeof(WorkFlowSchemeinfo))]
    public class UpdateWorkFlowSchemeinfoInput :EntityDto<Guid>,  IRemark, IPassivable, IDisplayOrder
    {
        #region 实体成员
        /// <summary> 
        /// 流程编码 
        /// </summary> 
        [StringLength(50)]
        public string Code { get; set; }
        /// <summary> 
        /// 流程模板名称 
        /// </summary> 
        [StringLength(200)]
        public string Name { get; set; }
        /// <summary> 
        /// 流程分类 
        /// </summary> 
        [StringLength(50)]
        public string Category { get; set; }

        /// <summary>
        /// 模板颜色
        /// </summary>
        [StringLength(50)]
        public string Color { get; set; }
        /// <summary>
        /// 模板图标
        /// </summary>
        [StringLength(50)]
        public string Icon { get; set; }

        /// <summary>
        /// 模板图标地址
        /// </summary>
        [StringLength(225), Description("模板图标")]
        public string IconUrl { get; set; }
        /// <summary> 
        /// 流程模板ID 
        /// </summary> 
        public Guid SchemeId { get; set; }
        /// <summary> 
        /// 是否有效 
        /// </summary> 
        public bool IsActive { get; set; }
        /// <summary> 
        /// 是否在我的任务允许发起 1允许 2不允许 
        /// </summary> 
        public int? Mark { get; set; }
        /// <summary> 
        /// 是否在App上允许发起 1允许 2不允许 
        /// </summary> 
        public int? IsInApp { get; set; }
        /// <summary>
        /// 流程权限类型1.都能发起 2.指定用户可发起
        /// </summary>
        public int? AuthType { get; set; }
        /// <summary>
        /// 流程监控权限类型1.都有监控权限 2.指定用户可发起
        /// </summary>
        public int? MonitorAuthType { get; set; }

        /// <summary>
        /// 密级(0公开，1内部，2秘密，3机密，4绝密)
        /// </summary>
        public int? SecretLevel { get; set; }

        /// <summary> 
        /// 归属单位 
        /// </summary> 
        public Guid? CompanyId { get; set; }

        /// <summary>
        /// 1.外部流程 2.系统本身流程
        /// </summary>
        public int? IsOther { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(200)]
        public string Remark { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int DisplayOrder { get; set; }
        #endregion

        #region 实体成员 
        ///// <summary> 
        ///// 1.正式2.草稿 
        ///// </summary> 
        //public int? Type { get; set; }
        ///// <summary> 
        ///// 流程模板内容 
        ///// </summary> 
        //[StringLength(int.MaxValue)]
        //public string Content { get; set; }
        #endregion
    }
}
