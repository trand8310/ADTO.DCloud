using ADTO.DCloud.ExcelManager.Export;
using ADTOSharp.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ADTO.DCloud.Controllers
{
    /// <summary>
    ///  Excel 导出配置(导出数据)
    /// </summary>
    [ADTOSharpAuthorize]
    [Route("api/[controller]/[action]")]
    public class ExcelExportController : DCloudControllerBase
    {
        private readonly IExcelExportAppService _excelExportAppService;
        public ExcelExportController(
          IExcelExportAppService excelExportAppService)
        {
            _excelExportAppService = excelExportAppService;
        }

        /// <summary>
        /// Excel 数据导出
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ExecuteExport([FromBody] ExportDto input)
        {
            var file = await _excelExportAppService.ExecuteExportDataAsync(input.ExportCode, input.QueryParams);
            return File(
                file.FileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                file.FileName
            );
        }


        //public async Task<IActionResult> ExecuteExport(string exportCode, [FromQuery] Dictionary<string, string> queryParams)
        //{
        //    var file = await _excelExportAppService.ExecuteExportDataAsync(exportCode, queryParams);
        //    return File(
        //        file.FileBytes,
        //        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        //        file.FileName
        //    );
        //}
    }

    /// <summary>
    /// 导出查询参数
    /// </summary>
    public class ExportDto
    {
        /// <summary>
        /// 导出配置编码
        /// </summary>
        public string ExportCode { get; set; }

        /// <summary>
        /// 接口查询参数
        /// </summary>
        public Dictionary<string, string> QueryParams { get; set; }
    }
}
