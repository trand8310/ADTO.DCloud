using ADTO.DCloud.Authorization.Users.Dto;
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
    public class DepartmentJoinedDto : EntityDto<Guid>, IHasCreationTime
    {
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

        /// <summary>
        /// 公司Id
        /// </summary>
        public virtual Guid? CompanyId { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        public virtual string CompanyName { get; set; }
        /// <summary>
        /// 负责人
        /// </summary>
        public virtual UserLightDto ManagerUser { get; set; }

        public int Classification { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreationTime { get; set; }

    }
}
