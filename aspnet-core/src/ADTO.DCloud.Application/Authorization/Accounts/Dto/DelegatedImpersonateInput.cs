using System;

namespace ADTO.DCloud.Authorization.Accounts.Dto;

public class DelegatedImpersonateInput
{
    public Guid UserDelegationId { get; set; }
}
