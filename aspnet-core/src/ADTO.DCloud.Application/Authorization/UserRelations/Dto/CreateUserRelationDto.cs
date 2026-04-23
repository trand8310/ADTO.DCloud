using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Authorization.UserRelations.Dto
{
    public class CreateUserRelationDto
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public  string UserId { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 分类:1-角色2-岗位-3部门角色
        /// </summary>
        public int Category { get; set; }
        /// <summary>
        /// 用户关系List
        /// </summary>
        public List<CreateUserRelationInput> userRelationLlist { get; set; }
    }
}
