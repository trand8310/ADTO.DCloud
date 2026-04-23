

using System;

namespace ADTO.OA.Storage.Dto
{
    public class CreateFileInfoDto
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileToken { get; set; }
        /// <summary>
        /// 类别ID
        /// </summary>
        public Guid CategoryId { get; set; }

        /// <summary>
        /// 描述及备注
        /// </summary>
        public string Description { get; set; }

    }
}
