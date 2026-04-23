using ADTOSharp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Authorization.Posts.Dto
{
    /// <summary>
    /// 成员关系
    /// </summary>
    public class UserPostInputDto
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid PostId { get; set; }
        /// <summary>
        /// 用户id字段
        /// </summary>
        public List<Guid> UserIds { get; set; }
    }

}
