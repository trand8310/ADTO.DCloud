using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ADTOSharp.AspNetCore.Mvc.Authorization;
using ADTOSharp.IO.Extensions;
using ADTOSharp.MimeTypes;
using ADTOSharp.Runtime.Session;
using ADTOSharp.UI;
using ADTOSharp.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ADTO.DCloud.Authorization;
using ADTO.DCloud.MultiTenancy;
using ADTO.DCloud.Storage;
using ADTO.DCloud.Controllers;
using ADTO.DCloud.Graphics;
using ADTO.DCloud.Net.MimeTypes;
using ADTO.DCloud.Tenants;
using ADTO.DCloud.Infrastructure;

namespace ADTO.DCloud.Web.Controllers;

/// <summary>
/// 租户自定义操作控制器,用于配置租户信息
/// </summary>
[ADTOSharpMvcAuthorize]
public class TenantCustomizationController : DCloudControllerBase
{
    private readonly TenantManager _tenantManager;
    private readonly IBinaryObjectManager _binaryObjectManager;
    private readonly IMimeTypeMap _mimeTypeMap;
    private readonly IImageValidator _imageValidator;

    public TenantCustomizationController(
        TenantManager tenantManager,
        IBinaryObjectManager binaryObjectManager,
        IMimeTypeMap mimeTypeMap,
        IImageValidator imageValidator)
    {
        _tenantManager = tenantManager;
        _binaryObjectManager = binaryObjectManager;
        _mimeTypeMap = mimeTypeMap;
        _imageValidator = imageValidator;
    }

    [HttpPost]
    [ADTOSharpMvcAuthorize(PermissionNames.Pages_Administration_Tenant_Settings)]
    public async Task<JsonResult> UploadLightLogo()
    {
        try
        {
            var logoObject = await UploadLogoFileInternal();

            var tenant = await _tenantManager.GetByIdAsync(ADTOSharpSession.GetTenantId());
            tenant.LightLogoId = logoObject.id;
            tenant.LightLogoFileType = logoObject.contentType;

            return Json(new AjaxResponse(new
            {
                id = logoObject.id,
                TenantId = tenant.Id,
                fileType = tenant.LightLogoFileType
            }));
        }
        catch (UserFriendlyException ex)
        {
            return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
        }
    }

    [HttpPost]
    [ADTOSharpMvcAuthorize(PermissionNames.Pages_Administration_Tenant_Settings)]
    public async Task<JsonResult> UploadDarkLogo()
    {
        try
        {
            var logoObject = await UploadLogoFileInternal();

            var tenant = await _tenantManager.GetByIdAsync(ADTOSharpSession.GetTenantId());
            tenant.DarkLogoId = logoObject.id;
            tenant.DarkLogoFileType = logoObject.contentType;

            return Json(new AjaxResponse(new
            {
                id = logoObject.id,
                TenantId = tenant.Id,
                fileType = tenant.DarkLogoFileType
            }));
        }
        catch (UserFriendlyException ex)
        {
            return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
        }
    }

    private async Task<(Guid id, string contentType)> UploadLogoFileInternal()
    {
        var logoFile = Request.Form.Files.First();

        //Check input
        if (logoFile == null)
        {
            throw new UserFriendlyException(L("File_Empty_Error"));
        }

        if (logoFile.Length > 102400) //100KB
        {
            throw new UserFriendlyException(L("File_SizeLimit_Error"));
        }

        byte[] fileBytes;
        using (var stream = logoFile.OpenReadStream())
        {
            fileBytes = stream.GetAllBytes();
            _imageValidator.ValidateDimensions(fileBytes, 512, 128);
        }

        var logoObject = new BinaryObject(ADTOSharpSession.GetTenantId(), fileBytes, $"Logo {DateTime.UtcNow}");
        await _binaryObjectManager.SaveAsync(logoObject);
        return (logoObject.Id, logoFile.ContentType);
    }

    [HttpPost]
    [ADTOSharpMvcAuthorize(PermissionNames.Pages_Administration_Tenant_Settings)]
    public async Task<JsonResult> UploadCustomCss()
    {
        try
        {
            var cssFile = Request.Form.Files.First();

            //Check input
            if (cssFile == null)
            {
                throw new UserFriendlyException(L("File_Empty_Error"));
            }

            if (cssFile.Length > 1048576) //1MB
            {
                throw new UserFriendlyException(L("File_SizeLimit_Error"));
            }

            byte[] fileBytes;
            using (var stream = cssFile.OpenReadStream())
            {
                fileBytes = stream.GetAllBytes();
            }

            var cssFileObject = new BinaryObject(ADTOSharpSession.GetTenantId(), fileBytes,
                $"Custom Css {cssFile.FileName} {DateTime.UtcNow}");
            await _binaryObjectManager.SaveAsync(cssFileObject);

            var tenant = await _tenantManager.GetByIdAsync(ADTOSharpSession.GetTenantId());
            tenant.CustomCssId = cssFileObject.Id;

            return Json(new AjaxResponse(new { id = cssFileObject.Id, TenantId = tenant.Id }));
        }
        catch (UserFriendlyException ex)
        {
            return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
        }
    }

    [AllowAnonymous]
    public async Task<ActionResult> GetLogo(Guid? tenantId)
    {
        if (tenantId == null)
        {
            tenantId = ADTOSharpSession.TenantId;
        }

        if (!tenantId.HasValue)
        {
            return StatusCode((int)HttpStatusCode.NotFound);
        }

        var tenant = await _tenantManager.FindByIdAsync(tenantId.Value);
        if (tenant == null || !tenant.HasLogo())
        {
            return StatusCode((int)HttpStatusCode.NotFound);
        }

        using (CurrentUnitOfWork.SetTenantId(tenantId.Value))
        {
            var logoObject = await _binaryObjectManager.GetOrNullAsync(tenant.LightLogoId.Value);
            if (logoObject == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound);
            }

            return File(logoObject.Bytes, tenant.LightLogoFileType);
        }
    }

    [HiddenApi]
    [AllowAnonymous]
    [Route("/TenantCustomization/GetTenantLogo/{skin}/{tenantId?}/{extension?}")]
    [HttpGet]
    public Task<ActionResult> GetTenantLogoWithCustomRoute(string skin, Guid? tenantId = null,
        string extension = "svg")
    {
        return GetTenantLogo(skin, tenantId, extension);
    }

    [HiddenApi,AllowAnonymous]
    public async Task<ActionResult> GetTenantLogo(string skin, Guid? tenantId, string extension = "svg")
    {
        var mimeType = _mimeTypeMap.GetMimeType("." + extension);
        var defaultLogo = "/Common/Images/app-logo-on-" + skin + "." + extension;

        if (tenantId == null)
        {
            return File(defaultLogo, mimeType);
        }

        var tenant = await _tenantManager.FindByIdAsync(tenantId.Value);
        if (tenant == null || !tenant.HasLogo())
        {
            return File(defaultLogo, mimeType);
        }

        async Task<ActionResult> GetLogoInternal(Guid id, string logoFileType)
        {
            var logoObject = await _binaryObjectManager.GetOrNullAsync(id);
            if (logoObject == null)
            {
                return File(defaultLogo, mimeType);
            }

            return File(logoObject.Bytes, logoFileType);
        }

        using (CurrentUnitOfWork.SetTenantId(tenantId.Value))
        {
            if (skin.ToLower() == "dark" || skin.ToLower() == "dark-sm")
            {
                if (tenant.HasDarkLogo())
                {
                    return await GetLogoInternal(tenant.DarkLogoId.Value, tenant.DarkLogoFileType);
                }

                if (tenant.HasLightLogo())
                {
                    return await GetLogoInternal(tenant.LightLogoId.Value, tenant.LightLogoFileType);
                }
            }
            else
            {
                if (tenant.HasLightLogo())
                {
                    return await GetLogoInternal(tenant.LightLogoId.Value, tenant.LightLogoFileType);
                }

                if (tenant.HasDarkLogo())
                {
                    return await GetLogoInternal(tenant.DarkLogoId.Value, tenant.DarkLogoFileType);
                }
            }
        }

        return File(defaultLogo, mimeType);
    }

    [AllowAnonymous]
    public async Task<ActionResult> GetTenantLogoOrNull(string skin, Guid tenantId)
    {
        var tenant = await _tenantManager.FindByIdAsync(tenantId);
        if (tenant == null || !tenant.HasLogo())
        {
            return Ok(new GetTenantLogoOutput());
        }

        async Task<ActionResult> GetLogoInternal(Guid id, string logoFileType)
        {
            var logoObject = await _binaryObjectManager.GetOrNullAsync(id);
            if (logoObject == null)
            {
                return null;
            }

            return Ok(new GetTenantLogoOutput(Convert.ToBase64String(logoObject.Bytes), logoFileType));
        }

        using (CurrentUnitOfWork.SetTenantId(tenantId))
        {
            if (skin.ToLower() == "dark" || skin.ToLower() == "dark-sm")
            {
                if (tenant.HasDarkLogo())
                {
                    return await GetLogoInternal(tenant.DarkLogoId.Value, tenant.DarkLogoFileType);
                }

                if (tenant.HasLightLogo())
                {
                    return await GetLogoInternal(tenant.LightLogoId.Value, tenant.LightLogoFileType);
                }
            }
            else
            {
                if (tenant.HasLightLogo())
                {
                    return await GetLogoInternal(tenant.LightLogoId.Value, tenant.LightLogoFileType);
                }

                if (tenant.HasDarkLogo())
                {
                    return await GetLogoInternal(tenant.DarkLogoId.Value, tenant.DarkLogoFileType);
                }
            }
        }

        return Ok(new GetTenantLogoOutput());
    }

    [AllowAnonymous]
    public async Task<ActionResult> GetCustomCss(Guid? tenantId)
    {
        if (tenantId == null)
        {
            tenantId = ADTOSharpSession.TenantId;
        }

        if (!tenantId.HasValue)
        {
            return StatusCode((int)HttpStatusCode.NotFound);
        }

        var tenant = await _tenantManager.FindByIdAsync(tenantId.Value);
        if (tenant == null || !tenant.CustomCssId.HasValue)
        {
            return StatusCode((int)HttpStatusCode.NotFound);
        }

        using (CurrentUnitOfWork.SetTenantId(tenantId.Value))
        {
            var cssFileObject = await _binaryObjectManager.GetOrNullAsync(tenant.CustomCssId.Value);
            if (cssFileObject == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound);
            }

            return File(cssFileObject.Bytes, MimeTypeNames.TextCss);
        }
    }
}
