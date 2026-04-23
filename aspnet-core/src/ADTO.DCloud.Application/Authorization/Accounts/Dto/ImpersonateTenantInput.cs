using System;
 

namespace ADTO.DCloud.Authorization.Accounts.Dto;

public class ImpersonateTenantInput
{
    public Guid? TenantId { get; set; }

    public Guid UserId { get; set; }
}