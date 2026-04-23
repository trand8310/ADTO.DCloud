using ADTOSharp.AspNetCore.Mvc.Authorization;
using ADTO.DCloud.Authorization.Users.Profile;
using ADTO.DCloud.Graphics;
using ADTO.DCloud.Storage;
using Microsoft.AspNetCore.Mvc;
using ADTOSharp;

namespace ADTO.DCloud.Web.Controllers;

/// <summary>
/// 用户偏好配置
/// </summary>
[ADTOSharpMvcAuthorize]
[Route("api/[controller]/[action]")]
public class ProfileController : ProfileControllerBase
{
    public ProfileController(
        ITempFileCacheManager tempFileCacheManager,
        IProfileAppService profileAppService,
        IImageValidator imageValidator, 
        IGuidGenerator guidGenerator) :
        base(tempFileCacheManager, profileAppService, imageValidator, guidGenerator)
    {
    }
}