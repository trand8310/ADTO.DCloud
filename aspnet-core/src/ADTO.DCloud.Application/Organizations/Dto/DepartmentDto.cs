using ADTO.DCloud.Authorization.Users.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using ADTOSharp.Domain.Entities.Auditing;
using ADTOSharp.Organizations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Organizations.Dto
{

    [AutoMapFrom(typeof(OrganizationUnitDto))]
    public class DepartmentDto : EntityDto<Guid>, IHasCreationTime
    {
        public DepartmentDto()
        {
            Children = new List<DepartmentDto>();
        }
        /// <summary>
        /// 名称
        /// </summary>
        public virtual string DisplayName { get; set; }
        /// <summary>
        /// 上级Id
        /// </summary>
        public virtual Guid? ParentId { get; set; }
        /// <summary>
        /// 组织代码 
        /// </summary>
        public virtual string Code { get; set; }

        public virtual ICollection<DepartmentDto> Children { get; set; }

        /// <summary>
        /// 公司Id
        /// </summary>
        public virtual Guid? CompanyId { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        public virtual string CompanyName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreationTime { get; set; }

    }
}
