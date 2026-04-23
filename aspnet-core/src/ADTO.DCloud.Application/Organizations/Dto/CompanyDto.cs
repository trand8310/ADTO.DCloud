using ADTO.DCloud.Authorization.Users.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using ADTOSharp.Domain.Entities.Auditing;
using ADTOSharp.Organizations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Organizations.Dto
{
    [AutoMapFrom(typeof(OrganizationUnitDto))]
    public class CompanyDto : EntityDto<Guid>, IHasCreationTime
    {
        public CompanyDto()
        {
            Children = new List<CompanyDto>();
        }
        public virtual string DisplayName { get; set; }
        public virtual string Code { get; set; }
        public virtual Guid? ParentId { get; set; }
        public virtual ICollection<CompanyDto> Children { get; set; }
        /// <summary>
        /// 负责人
        /// </summary>
        public virtual UserLightDto? ManagerUser { get; set; }
        public virtual DateTime CreationTime { get; set; }
    }
}
