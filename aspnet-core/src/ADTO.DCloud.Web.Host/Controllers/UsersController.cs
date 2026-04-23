using ADTOSharp.AspNetCore.Mvc.Authorization;
using ADTO.DCloud.Authorization;
using ADTO.DCloud.Storage;
using ADTOSharp.BackgroundJobs;
 

namespace ADTO.DCloud.Web.Controllers;

[ADTOSharpMvcAuthorize(PermissionNames.Pages_Administration_Users)]
public class UsersController : UsersControllerBase
{
    public UsersController(IBinaryObjectManager binaryObjectManager, IBackgroundJobManager backgroundJobManager)
        : base(binaryObjectManager, backgroundJobManager)
    {
    }
}