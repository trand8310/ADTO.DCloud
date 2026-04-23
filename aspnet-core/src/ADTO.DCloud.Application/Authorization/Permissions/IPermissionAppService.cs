using ADTOSharp.Application.Services;
using ADTOSharp.Application.Services.Dto;
using ADTO.DCloud.Authorization.Permissions.Dto;

namespace ADTO.DCloud.Authorization.Permissions
{
    public interface IPermissionAppService : IApplicationService
    {
        ListResultDto<FlatPermissionWithLevelDto> GetAllPermissions();
    }
}
