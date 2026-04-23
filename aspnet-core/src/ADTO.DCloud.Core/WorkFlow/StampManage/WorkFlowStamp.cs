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

namespace ADTO.DCloud.WorkFlow.StampManage
{
    /// <summary>
    /// 印章管理
    /// </summary>
    [Description("印章管理"),Table("WorkFlowStamps")]
    public class WorkFlowStamp: FullAuditedAggregateRoot<Guid>,IRemark, IPassivable,IMayHaveTenant
    {
        #region 实体成员
        /// <summary>
        /// 印章名称
        /// </summary>
        [StringLength(100),Description("印章名称")]
        public string StampName { get; set; }
        /// <summary>
        /// 印章分类
        /// </summary>
        [StringLength(100), Description("印章分类")]
        public string StampType { get; set; }
        /// <summary>
        /// 是否启用密码 1 是 0 不是
        /// </summary>
        [Description("是否启用密码 1 是 0 不是")]
        public int? IsNotPassword { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [StringLength(50), Description("密码")]
        public string Password { get; set; }
        /// <summary>
        /// 图片文件
        /// </summary>
        [StringLength(100), Description("图片文件")]
        public string ImgFile { get; set; }
        /// <summary>
        /// 关联用户（默认是创建用户）
        /// </summary>
        [StringLength(500), Description("关联用户（默认是创建用户）")]
        public string UserIds { get; set; }
        /// <summary>
        /// 印章状态
        /// </summary>
        [Description("印章状态")]
        public bool IsActive { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [StringLength(200),Description("描述")]
        public string Remark { get; set; }
        #endregion
        #region 多租户

        /// <summary>
        /// 多租户
        /// </summary>
        [Description("多租户")]
        public Guid? TenantId { get; set; }
        #endregion
    }
}
