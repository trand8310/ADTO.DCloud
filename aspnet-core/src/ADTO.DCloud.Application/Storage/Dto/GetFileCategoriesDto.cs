using ADTOSharp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Storage.Dto
{
    public class GetFileCategoriesDto
    {
        /// <summary>
        /// 文件类别
        /// </summary>
        public SharedFileCategory? SharedFileCategory { get; set; }
        /// <summary>
        /// 文件权限
        /// </summary>
        public SharedFileAuthorizes? SharedFileAuthorize { get; set; }
    }
}
