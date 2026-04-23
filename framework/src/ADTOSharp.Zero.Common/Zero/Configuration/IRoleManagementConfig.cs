using System.Collections.Generic;

namespace ADTOSharp.Zero.Configuration
{
    public interface IRoleManagementConfig
    {
        List<StaticRoleDefinition> StaticRoles { get; }
    }
}