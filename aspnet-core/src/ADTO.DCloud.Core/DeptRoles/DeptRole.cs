using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.DeptRoles
{
    /// <summary>
    /// (部门角色表)表的实体
    /// </summary>
    public class DeptRole : FullAuditedEntity<Guid>, IRemark, IPassivable, IMayHaveTenant, IDisplayOrder
    {
        #region 实体成员
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 分类
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int DisplayOrder { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public bool IsActive { get; set; }
        #endregion
        #region 多租户
        /// <summary>
        /// 多租户
        /// </summary>
        public Guid? TenantId { get; set; }
        #endregion
    }
}
