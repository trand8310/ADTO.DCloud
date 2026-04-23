using ADTO.DCloud.Authorization.Roles;
using ADTO.DCloud.MultiTenancy;
using ADTOSharp.Organizations;
using System;
using System.Linq;
 

namespace ADTO.DCloud.EntityHistory;

public static class EntityHistoryHelper
{
    public const string EntityHistoryConfigurationName = "EntityHistory";

    public static readonly Type[] HostSideTrackedTypes =
    {
        typeof(OrganizationUnit), typeof(Role), typeof(Tenant)
    };

    public static readonly Type[] TenantSideTrackedTypes =
    {
        typeof(OrganizationUnit), typeof(Role)
    };

    public static readonly Type[] TrackedTypes =
        HostSideTrackedTypes
            .Concat(TenantSideTrackedTypes)
            .GroupBy(type => type.FullName)
            .Select(types => types.First())
            .ToArray();
}
