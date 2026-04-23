using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Authorization.UserRelations
{
    /// <summary>
    /// 用户关联对象
    /// </summary>
    [Table("UserRelations")]
    public class UserRelation : Entity<Guid>, ICreationAudited, IMayHaveTenant
    {
        #region 实体成员
        /// <summary>
        /// 用户主键
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 分类:1-角色2-岗位-3部门角色
        /// </summary>
        public int? Category { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 组织架构扩展角色属性编码或者岗位主岗或者兼容岗
        /// </summary>
        public string AttrCode { get; set; }
        /// <summary>
        /// 分类 userid 代表的意义 1.用户 2.部门 3.密级 4.行政级别 5.行政职务 6.技术职务 7.职称等级 8.管理职务 9.集成角色  10.限定部门 11.排除 部门
        /// </summary>
        public int? Type { get; set; }

        /// <summary>
        /// 对象主键
        /// </summary>
        public string ObjectId { get; set; }
        #endregion

        #region 多租户
        /// <summary>
        /// 租户ID
        /// </summary>
        public Guid? TenantId { get; set; }
        public Guid? CreatorUserId { get; set; }
        public DateTime CreationTime { get; set; }
        #endregion
    }
}
