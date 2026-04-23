using ADTOSharp.Application.Services.Dto;

namespace ADTO.DCloud.ExcelManager.Export.Dto
{
    public class PagedExcelExportResultRequestDto : PagedResultRequestDto
    {
        /// <summary>
        /// 关键词
        /// </summary>
        public string KeyWord { get; set; }
    }
}
