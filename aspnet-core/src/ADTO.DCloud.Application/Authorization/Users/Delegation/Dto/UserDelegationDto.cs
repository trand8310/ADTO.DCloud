using System;
using ADTOSharp.Application.Services.Dto;

namespace ADTO.DCloud.Authorization.Users.Delegation.Dto
{
    public class UserDelegationDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid DepartmentId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid CompanyId { get; set; }
    }
}