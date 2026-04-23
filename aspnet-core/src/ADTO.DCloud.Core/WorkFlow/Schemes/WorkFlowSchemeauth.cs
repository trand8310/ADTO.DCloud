using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Schemes
{
    /// <summary>
    /// 工作流模板权限
    /// </summary>
    [Description("工作流模板权限"),Table("WorkFlowSchemeauths")]
    public class WorkFlowSchemeauth : Entity<Guid>, ICreationAudited, IMayHaveTenant
    {
        #region 实体成员 
        /// <summary> 
        /// 流程模板信息主键 
        /// </summary> 
        [Description("流程模板信息主键")]
        public Guid? SchemeInfoId { get; set; }
        /// <summary> 
        /// 对象名称 
        /// </summary> 
        [Description("对象名称"), StringLength(100)]
        public string ObjName { get; set; }
        /// <summary> 
        /// 对应对象主键 
        /// </summary> 
        [Description("对应对象主键")]
        public Guid? ObjId { get; set; }
        /// <summary> 
        /// 对应对象类型1岗位2角色3用户4所用人可看 
        /// </summary> 
        [Description("对应对象类型1岗位2角色3用户4所用人可看")]
        public int? ObjType { get; set; }

        /// <summary> 
        /// 类型 2 监控 1和空为发起权限
        /// </summary> 
        [Description("类型 2 监控 1和空为发起权限")]
        public int? Type { get; set; }


        #endregion
        #region 多租户
        public Guid? CreatorUserId { get; set; }
        public DateTime CreationTime {  get; set; }
        /// <summary>
        /// 多租户
        /// </summary>
        public Guid? TenantId { get; set; }
        #endregion
    }
}
