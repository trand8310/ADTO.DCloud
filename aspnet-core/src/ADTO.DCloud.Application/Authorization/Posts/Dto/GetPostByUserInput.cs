using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Authorization.Posts.Dto
{
    /// <summary>
    /// 根据用户ID获取岗位信息
    /// </summary>
    public class GetPostByUserInput
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public Guid UserId { get; set; }
    }
}
