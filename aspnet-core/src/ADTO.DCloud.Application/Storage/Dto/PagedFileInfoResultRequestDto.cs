
using ADTOSharp.Application.Services.Dto;
using System;

namespace ADTO.DCloud.Storage.Dto
{
    public class PagedFileInfoResultRequestDto : PagedResultRequestDto
    {
        /// <summary>
        /// 文件类别ID
        /// </summary>
        public Guid? CategoryId { get; set; }
        /// <summary>
        /// 关键词
        /// </summary>
        public string Keyword { get; set; }

    }
}
