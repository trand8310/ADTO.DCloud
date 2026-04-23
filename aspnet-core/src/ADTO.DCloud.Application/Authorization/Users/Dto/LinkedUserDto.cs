using System;
using ADTOSharp.Application.Services.Dto;

namespace ADTO.DCloud.Authorization.Users.Dto
{
    public class LinkedUserDto : EntityDto<Guid>
    {
        public Guid? TenantId { get; set; }

        public string TenancyName { get; set; }

        public string Username { get; set; }

        public object GetShownLoginName(bool multiTenancyEnabled)
        {
            if (!multiTenancyEnabled)
            {
                return Username;
            }

            return string.IsNullOrEmpty(TenancyName)
                ? ".\\" + Username
                : TenancyName + "\\" + Username;
        }
    }
}