using ADTOSharp.Application.Services.Dto;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Authorization.UserRelations.Dto
{
    public class CreateUserRelationInput 
    {
        #region 实体成员
        /// <summary>
        /// 组织架构扩展角色属性编码或者岗位主岗或者兼容岗
        /// </summary>
        public string AttrCode { get; set; }
        /// <summary>
        /// 对象主键
        /// </summary>
        public string ObjectId { get; set; }
        #endregion
    }
}
