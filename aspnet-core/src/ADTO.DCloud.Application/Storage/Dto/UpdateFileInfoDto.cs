

using ADTOSharp.Application.Services.Dto;
using System;

namespace ADTO.OA.Storage.Dto
{
    /// <summary>
    /// UpdateFileInfoDto
    /// </summary>
    public class UpdateFileInfoDto : EntityDto<Guid>
    {
        /// <summary>
        /// 类别ID
        /// </summary>
        public Guid? CategoryId { get; set; }
        /// <summary>
        /// 描述及备注
        /// </summary>
        public string Description { get; set; }

    }
}
