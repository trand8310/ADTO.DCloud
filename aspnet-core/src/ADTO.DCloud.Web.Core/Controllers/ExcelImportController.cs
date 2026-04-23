using ADTO.DCloud.ExcelManager.Import;
using ADTO.DCloud.ExcelManager.Import.Dto;
using ADTOSharp.Authorization;
using ADTOSharp.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace ADTO.DCloud.Controllers
{
    /// <summary>
    /// Excel 导入配置(导入数据、导出模板)
    /// </summary>
    //[ADTOSharpAuthorize]
    [Route("api/[controller]/[action]")]
    public class ExcelImportController : DCloudControllerBase
    {
        //文件相关处理，封装一层控制器，也可直接AppService处理

        private readonly IExcelImportAppService _excelImportAppService;
        public ExcelImportController(
          IExcelImportAppService excelImportAppService)
        {
            _excelImportAppService = excelImportAppService;
        }

        /// <summary>
        /// 导出模板（Excel导入模板）
        /// </summary>
        /// <param name="importCode"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> DownloadTemplateAsync(string importCode)
        {
            var excelBytes = await _excelImportAppService.DownloadImportTemplateAsync(importCode);
 
            return new FileContentResult(
                excelBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            )
            {
                FileDownloadName = $"导入模板_{importCode}.xlsx"
            };
        }

        /// <summary>
        /// 执行Excel导入
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ExecuteImportAsync([FromForm] ImportExcelRequest input)
        {
            // 1. 验证文件
            if (input.File == null || input.File.Length == 0)
            {
                throw new UserFriendlyException("请上传有效的Excel文件");
            }

            // 2. 验证文件扩展名
            var extension = Path.GetExtension(input.File.FileName).ToLower();
            if (extension != ".xlsx" && extension != ".xls")
            {
                throw new UserFriendlyException("仅支持.xls和.xlsx格式的Excel文件");
            }

            // 3. 将 IFormFile 转换为 byte[]
            byte[] fileBytes;
            using (var ms = new MemoryStream())
            {
                await input.File.CopyToAsync(ms);
                fileBytes = ms.ToArray();
            }

            // 4. 调用应用层
            var result = await _excelImportAppService.ExecuteExcelImportAsync(new ExecuteExcelImportDto
            {
                ImportCode = input.ImportCode,
                FileContent = fileBytes,
                FileName = input. File.FileName
            });

            // 5. 返回结果
            return Ok(result);
        }
    }

    /// <summary>
    /// 导入参数视图
    /// </summary>
    public class ImportExcelRequest
    {
        /// <summary>
        /// 模板编码
        /// </summary>
        public string ImportCode { get; set; }

        //文件
        public IFormFile File { get; set; }
    }
}
