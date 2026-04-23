using System;
using ADTOSharp.AutoMapper;
using ADTOSharp.Application.Services.Dto;

namespace ADTO.DCloud.FormScheme.Dto
{
    /// <summary>
    /// 添加动态表单记录
    /// </summary>
    public class CreateFormSchemeInputDto
    {
        /// <summary>
        /// 模板基础信息
        /// </summary>
        public FormSchemeInfoDto? Info { get; set; }
        /// <summary>
        /// 模板信息
        /// </summary>
        public FormSchemeDto? Scheme { get; set; }
    }
}
