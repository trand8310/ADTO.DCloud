using ADTO.DCloud.Dto;
using ADTO.DCloud.Infrastructure;
using ADTO.DCloud.Media.UploadFiles.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.UI;
using ADTOSharp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ADTO.DCloud.Media.UploadFiles
{
    /// <summary>
    /// 文件上传
    /// </summary>
    public interface IUploadFileAppService
    {
        /// <summary>
        /// 从内存中取图片信息，保存文件（分两步上传图片，这是第二步  表单方式，后端方法内部调用）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        Task<AjaxResponse<object>> UploadFileAsync(UploadSaveFileDto input);

        /// <summary>
        /// 根据FolderId查询图片集合
        /// </summary>
        /// <param name="FolderId"></param>
        /// <returns></returns>
        Task<List<UploadFileDto>> GetFileListByFolderId(Guid FolderId);

        /// <summary>
        /// 根据FolderId查询图片集合,包含图片地址
        /// </summary>
        /// <param name="FolderId"></param>
        /// <returns></returns>
        Task<List<UploadFileDto>> GetFileUrlListByFolderId(Guid FolderId);

        /// <summary>
        /// 删除图片（逻辑删除）
        /// </summary>
        /// <param name="input">图片Id</param>
        /// <returns></returns>
        Task DeleteUploadFileAsync(EntityDto<Guid> input);

        /// <summary>
        /// 批量删除，根据图片Id
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task DeleteUploadFileListAsync(List<Guid> input);

        /// <summary>
        /// 批量删除，根据FolderId
        /// </summary>
        /// <param name="FolderId"></param>
        /// <returns></returns>
        Task DeleteUploadFileByFolderIdAsync(Guid FolderId);
        /// <summary>
        /// 删除附件
        /// </summary>
        /// <param name="folderId"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        Task DeleteUploadFileByFolderId2Async(Guid folderId, List<UploadFilesInputDto> list);
        /// <summary>
        /// 上传附件-20260115新增
        /// </summary>
        /// <param name="folderId"></param>
        /// <param name="list"></param>
        /// <param name="projectId"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        Task<string> UploadFolderFileAsync(string folderId, List<UploadFilesInputDto> list, Guid? projectId = null, Guid? entityId = null);

        /// <summary>
        /// 上传文件至阿里云（图库上传）
        /// </summary>
        /// <returns></returns>
        Task<AjaxResponse<object>> UploadFilesUEditAsync();
        string GetUEditorConfiguration();
        /// <summary>
        /// Get the string to write an error response
        /// </summary>
        /// <param name="message">Additional message</param>
        /// <returns>String to write to the response</returns>
        string GetErrorResponse(string message = null);
        /// <summary>
        /// Whether the request is made with ajax 
        /// </summary>
        /// <returns>True or false</returns>
        bool IsAjaxRequest();
    }
}
