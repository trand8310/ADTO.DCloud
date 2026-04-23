using ADTO.DCloud.Authorization.Users;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;

namespace ADTO.DCloud.Organizations.Dto
{
    [AutoMapFrom(typeof(User))]
    public class OrganizationUnitUserListDto : EntityDto<Guid>
    {
        public string Name { get; set; }

        public string UserName { get; set; }

        public string EmailAddress { get; set; }

        public Guid? ProfilePictureId { get; set; }

        public DateTime AddedTime { get; set; }
    }
}