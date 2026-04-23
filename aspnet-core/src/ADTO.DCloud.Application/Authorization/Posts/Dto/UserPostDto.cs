using ADTO.DCloud.Authorization.Posts;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using ADTOSharp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Authorization.Posts.Dto
{
    [AutoMap(typeof(UserPost))]
    public class UserPostDto : EntityDto<Guid>, ICreationAudited
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

        /// <summary>
        /// 排序
        /// </summary>
        public int DisplayOrder { get; set; }
        #endregion

        public Guid? CreatorUserId { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
