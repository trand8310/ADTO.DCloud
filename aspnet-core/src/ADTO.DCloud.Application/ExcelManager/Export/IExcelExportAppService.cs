using ADTO.DCloud.ExcelManager.Export.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ADTO.DCloud.ExcelManager.Export
{
    /// <summary>
    /// excel 导出
    /// </summary>
    public interface IExcelExportAppService
    {
        /// <summary>
        /// 数据导出
        /// </summary>
        /// <param name="exportCode"></param>
        /// <param name="queryParams"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        Task<ExcelFileDto> ExecuteExportDataAsync(string exportCode, Dictionary<string, string> queryParams);
    }
}
