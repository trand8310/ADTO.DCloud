

using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;

namespace ADTO.DCloud.Storage.Dto
{
    /// <summary>
    /// 文件目录,Nested
    /// </summary>
    [AutoMapFrom(typeof(SharedFileCategory))]
    public class FileCategoryLiteDto : EntityDto<Guid>
    {
        /// <summary>
        /// 目录名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

    }
}
