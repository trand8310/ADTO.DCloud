using ADTOSharp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Authorization.Posts.Dto
{
    /// <summary>
    /// 修改用户岗位排序码
    /// </summary>
    public class UpdateUserPostDisplayOrderDto
    {
        /// <summary>
        /// 岗位Id
        /// </summary>
        public Guid PostId { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int DisplayOrder { get; set; }
    }
}
