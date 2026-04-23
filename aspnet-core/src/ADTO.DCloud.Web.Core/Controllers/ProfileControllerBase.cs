using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ADTOSharp.Extensions;
using ADTOSharp.IO.Extensions;
using ADTOSharp.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ADTO.DCloud.Authorization.Users.Profile;
using ADTO.DCloud.Dto;
using ADTO.DCloud.Storage;
using ADTO.DCloud.Controllers;
using ADTO.DCloud.Graphics;
using ADTO.DCloud.Net.MimeTypes;
using System.Collections.Generic;
using ADTO.DCloud.Infrastructure;
using ADTOSharp;
using Microsoft.AspNetCore.Http;
using NuGet.Protocol.Plugins;

namespace ADTO.DCloud.Web.Controllers;
/// <summary>
/// 用户偏好设置基类,目前主要处理用户图像
/// </summary>
public abstract class ProfileControllerBase : DCloudControllerBase
{
    private readonly ITempFileCacheManager _tempFileCacheManager;
    private readonly IProfileAppService _profileAppService;
    private readonly IImageValidator _imageValidator;
    private readonly IGuidGenerator _guidGenerator;

    private const int MaxProfilePictureSize = 5242880; //5MB

    protected ProfileControllerBase(
        ITempFileCacheManager tempFileCacheManager,
        IProfileAppService profileAppService, 
        IImageValidator imageValidator,
        IGuidGenerator guidGenerator)
    {
        _tempFileCacheManager = tempFileCacheManager;
        _profileAppService = profileAppService;
        _imageValidator = imageValidator;
        _guidGenerator = guidGenerator;
    }
    [HttpPost]
    public ActionResult<FileDto> UploadProfilePicture()
    {
        var profilePictureFile = Request.Form.Files.First();

        //Check input
        if (profilePictureFile == null)
        {
            throw new UserFriendlyException(L("ProfilePicture_Change_Error"));
        }

        if (profilePictureFile.Length > MaxProfilePictureSize)
        {
            throw new UserFriendlyException(L("ProfilePicture_Warn_SizeLimit",
                AppConsts.MaxProfilePictureBytesUserFriendlyValue));
        }

        var fileName = profilePictureFile.FileName;
        var fileType = CommonHelper.GetMimeTypeFromFileName(fileName);
        var fileToken = _guidGenerator.Create().ToString("N");


        byte[] fileBytes;
        using (var stream = profilePictureFile.OpenReadStream())
        {
            fileBytes = stream.GetAllBytes();
            _imageValidator.Validate(fileBytes);
        }

        _tempFileCacheManager.SetFile(fileToken, fileBytes);

        return new FileDto() { FileToken = fileToken, FileName = profilePictureFile.FileName };
    }

    [HttpGet,AllowAnonymous]
    public FileResult GetDefaultProfilePicture()
    {
        return GetDefaultProfilePictureInternal();
    }
    [HttpGet]
    public async Task<FileResult> GetProfilePictureByUser(Guid userId)
    {
        var output = await _profileAppService.GetProfilePictureByUser(userId);
        if (output.ProfilePicture.IsNullOrEmpty())
        {
            return GetDefaultProfilePictureInternal();
        }

        return File(Convert.FromBase64String(output.ProfilePicture), MimeTypeNames.ImageJpeg);
    }

    protected FileResult GetDefaultProfilePictureInternal()
    {
        return File(Path.Combine("Common", "Images", "default-profile-picture.png"), MimeTypeNames.ImagePng);
    }
}
