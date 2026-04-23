using ADTOSharp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.ExcelManager.Import.Dto
{
    public class PagedExcelImportResultRequestDto : PagedResultRequestDto
    {
        /// <summary>
        /// 关键词
        /// </summary>
        public string KeyWord { get; set; }
    }
}
