using ADTOSharp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Schemes.Dto
{
    /// <summary>
    /// 导出流程模板
    /// </summary>
    public class ExportSchemeInput:EntityDto<Guid>
    {
        /// <summary>
        /// 目录
        /// </summary>
        public string Path { get; set; }

    }
}
