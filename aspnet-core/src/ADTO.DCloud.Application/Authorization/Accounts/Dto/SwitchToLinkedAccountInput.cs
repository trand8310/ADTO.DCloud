using System;
using System.ComponentModel.DataAnnotations;
using ADTOSharp;

namespace ADTO.DCloud.Authorization.Accounts.Dto;

public class SwitchToLinkedAccountInput
{
    public Guid? TargetTenantId { get; set; }

    public Guid TargetUserId { get; set; }

    public UserIdentifier ToUserIdentifier()
    {
        return new UserIdentifier(TargetTenantId, TargetUserId);
    }
}
