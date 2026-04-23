using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Organizations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Organizations.Dto
{
    /// <summary>
    /// 编辑返回组织Dto
    /// </summary>
    [AutoMap(typeof(OrganizationUnit))]
    public class OrganizationUnitEditDto : AuditedEntityDto<Guid>
    {
        /// <summary>
        /// 上级组织-显示名称
        /// </summary>
        public string ParentName { get; set; }
        /// <summary>
        /// 上级组织
        /// </summary>
        public Guid? ParentId { get; set; }
        /// <summary>
        /// 组织代码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        ///  组织分级,默认:0,集群:1,集团:2,公司:3,部门:4,班/组:5,如果还存在其它的,在这个上面,可以进一步细分,具体解释规则由上层应用来确定
        /// </summary>
        public int Classification { get; set; }
        //
        // 摘要:
        //     公司/部门负责人Id
        public virtual Guid? ManagerUserId { get; set; }

        /// <summary>
        /// 公司/部门负责人
        /// </summary>
        public string ManagerUserName { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int DisplayOrder { get;set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public bool IsActive { get; set; }
    }
}
