using System;


namespace ADTO.DCloud.Authorization.Accounts.Dto;

public class ImpersonateUserInput
{
    public Guid? TenantId { get; set; }

    public Guid UserId { get; set; }
}
