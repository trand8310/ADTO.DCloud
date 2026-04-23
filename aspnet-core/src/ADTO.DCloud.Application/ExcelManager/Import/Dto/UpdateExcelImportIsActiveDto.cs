using ADTOSharp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.ExcelManager.Import.Dto
{
    /// <summary>
    /// 修改状态
    /// </summary>
    public class UpdateExcelImportIsActiveDto:EntityDto<Guid>
    {
        /// <summary>
        /// 启用状态
        /// </summary>
        public bool IsActive { get; set; }
    }
}
