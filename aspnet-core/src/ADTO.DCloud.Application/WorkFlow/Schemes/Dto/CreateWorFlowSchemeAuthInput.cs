using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Schemes.Dto
{
    [AutoMapTo(typeof(WorkFlowSchemeauth))]
    public class CreateWorFlowSchemeAuthInput
    {
        #region 实体成员 
        /// <summary> 
        /// 对象名称 
        /// </summary> 
        [StringLength(100)]
        public string ObjName { get; set; }
        /// <summary> 
        /// 对应对象主键 
        /// </summary> 
        public Guid? ObjId { get; set; }
        /// <summary> 
        /// 对应对象类型1岗位2角色3用户4所用人可看 
        /// </summary> 
        public int? ObjType { get; set; }

        /// <summary> 
        /// 类型 2 监控 1和空为发起权限
        /// </summary> 
        public int? Type { get; set; }

        #endregion
    }
}
