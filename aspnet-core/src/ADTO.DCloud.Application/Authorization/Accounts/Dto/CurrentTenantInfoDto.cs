using ADTOSharp.Application.Services.Dto;
using System;

namespace ADTO.DCloud.Authorization.Accounts.Dto;


public class CurrentTenantInfoDto : EntityDto<Guid>
{
    public string TenancyName { get; set; }

    public string Name { get; set; }
}
