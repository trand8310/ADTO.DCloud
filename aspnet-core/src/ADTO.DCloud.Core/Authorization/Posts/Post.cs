using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Authorization.Posts
{
    /// <summary>
    /// 岗位管理
    /// </summary>
    [Table("Posts")]
    public class Post : FullAuditedAggregateRoot<Guid>, IRemark, IDisplayOrder, IMayHaveTenant
    {
        #region 实体成员
        [ForeignKey("ParentId")]
        public Guid? ParentId { get; set; }
        /// <summary>
        /// 上级主键
        /// </summary>
        public  Post? Parent { get; set; }
        /// <summary>
        /// 岗位名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 岗位编号
        /// </summary>
        public string EnCode { get; set; }
        /// <summary>
        /// 公司主键
        /// </summary>
        public Guid CompanyId { get; set; }
        /// <summary>
        /// 部门主键
        /// </summary>
        public Guid DepartmentId { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int DisplayOrder { get; set; }
        /// <summary>
        /// 多租户
        /// </summary>
        public Guid? TenantId {  get; set; }
        #endregion

    }
}
