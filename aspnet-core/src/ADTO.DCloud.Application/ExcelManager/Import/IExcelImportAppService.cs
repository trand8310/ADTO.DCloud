using ADTO.DCloud.ExcelManager.Import.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.ExcelManager.Import
{
    /// <summary>
    /// EXcel 导入配置
    /// </summary>
    public interface IExcelImportAppService
    {
        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<ExcelImportResultDto> ExecuteExcelImportAsync([FromForm] ExecuteExcelImportDto input);

        /// <summary>
        /// 导出模板
        /// </summary>
        /// <param name="ImportCode"></param>
        /// <returns></returns>
        Task<byte[]> DownloadImportTemplateAsync(string ImportCode);
    }
}
