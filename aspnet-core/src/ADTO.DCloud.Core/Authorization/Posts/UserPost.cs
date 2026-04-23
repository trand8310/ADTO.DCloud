using ADTOSharp.Application.Services.Dto;
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
    /// 用户关联对象
    /// </summary>
    [Table("UserPosts")]
    public class UserPost : Entity<Guid>, ICreationAudited, IMayHaveTenant,IDisplayOrder
    {
        #region 实体成员
        /// <summary>
        /// 用户主键
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 岗位主键
        /// </summary>
        public Guid PostId { get; set; }
        #endregion

        #region 多租户
        /// <summary>
        /// 租户ID
        /// </summary>
        public Guid? TenantId { get; set; }
        public Guid? CreatorUserId { get; set; }
        public DateTime CreationTime { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int DisplayOrder { get; set; }
        #endregion
    }
}
