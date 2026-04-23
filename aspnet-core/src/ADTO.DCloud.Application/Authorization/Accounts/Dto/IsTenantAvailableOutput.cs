using System;

namespace ADTO.DCloud.Authorization.Accounts.Dto;

public class IsTenantAvailableOutput
{
    public TenantAvailabilityState State { get; set; }

    public Guid? TenantId { get; set; }

    public string ServerRootAddress { get; set; }

    public IsTenantAvailableOutput()
    {
        
    }

    public IsTenantAvailableOutput(TenantAvailabilityState state, Guid? tenantId = null)
    {
        State = state;
        TenantId = tenantId;
    }

    public IsTenantAvailableOutput(TenantAvailabilityState state, Guid? tenantId, string serverRootAddress)
    {
        State = state;
        TenantId = tenantId;
        ServerRootAddress = serverRootAddress;
    }
}