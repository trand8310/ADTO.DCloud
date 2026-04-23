using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Authorization.Posts.Dto
{

    public class GetByUserIdListByUserIdsInput
    {
        /// <summary>
        /// 对象Id集合
        /// </summary>
        public List<Guid> ObjectIdList { get; set; }
        /// <summary>
        /// 组织架构扩展角色属性编码或者岗位主岗或者兼容岗
        /// </summary>
        public string AttrCode { get; set; }
    }
}
