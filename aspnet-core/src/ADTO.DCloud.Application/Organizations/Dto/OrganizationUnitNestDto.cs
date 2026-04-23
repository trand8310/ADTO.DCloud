using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using ADTOSharp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Organizations.Dto
{

    [AutoMapFrom(typeof(OrganizationUnitDto))]
    public class OrganizationUnitNestDto : EntityDto<Guid>, IHasCreationTime
    {
        public OrganizationUnitNestDto()
        {
            Children = new List<OrganizationUnitNestDto>();
        }

        public virtual string DisplayName { get; set; }

        public virtual string Code { get; set; }
        public virtual Guid? ParentId { get; set; }
        public virtual ICollection<OrganizationUnitNestDto> Children { get; set; }
        public virtual DateTime CreationTime { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int DisplayOrder { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// 成员数量
        /// </summary>
        public int MemberCount { get; set; }
        //
        // 摘要:
        //     公司/部门负责人Id
        public virtual Guid? ManagerUserId { get; set; }
        /// <summary>
        /// 负责人
        /// </summary>
        public string ManagerUser { get; set; }
        /// <summary>
        /// 负责人联系电话
        /// </summary>
        public string ManagerUserPhoneNumber { get; set; }
    }
}
