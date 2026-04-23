using ADTOSharp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.ExcelManager.Export.Dto
{
    /// <summary>
    /// 修改启用状态
    /// </summary>
    public class UpdateExcelExportIsActiveDto : EntityDto<Guid>
    {
        /// <summary>
        /// 启用状态
        /// </summary>
        public bool IsActive { get; set; }
    }
}
