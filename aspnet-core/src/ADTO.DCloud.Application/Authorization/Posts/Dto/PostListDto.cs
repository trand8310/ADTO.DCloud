using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using ADTOSharp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Authorization.Posts.Dto
{
    /// <summary>
    /// 
    /// </summary>
    [AutoMapFrom(typeof(Post))]
    public class PostListDto:FullAuditedEntityDto<Guid>, IRemark, IDisplayOrder
    {
        #region 实体成员
        /// <summary>
        /// 上级主键
        /// </summary>
        public virtual Post? Parent { get; set; }
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
        #endregion
    }
}
