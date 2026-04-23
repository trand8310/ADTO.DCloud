using ADTOSharp.AutoMapper;
using ADTOSharp.Domain.Entities.Auditing;
using ADTOSharp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADTOSharp.Application.Services.Dto;

namespace ADTO.DCloud.Authorization.Posts.Dto
{

    /// <summary>
    /// 岗位树结构Dto
    /// </summary>
    [AutoMap(typeof(Post))]
    public class PostTreeDto : FullAuditedEntityDto<Guid>, IRemark, IDisplayOrder
    {
        #region 实体成员
        [ForeignKey("ParentId")]
        public Guid? ParentId { get; set; }
        /// <summary>
        /// 上级主键
        /// </summary>
        public Post? Parent { get; set; }
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
        /// 子节点
        /// </summary>
        public List<PostTreeDto> Children { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string DepartmentName { get; set; }

        /// <summary>
        /// 所属公司名称
        /// </summary>
        public string CompanyName { get; set; }
        #endregion

    }
}
