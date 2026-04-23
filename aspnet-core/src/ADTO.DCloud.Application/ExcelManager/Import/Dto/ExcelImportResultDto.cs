using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.ExcelManager.Import.Dto
{
    public class ExcelImportResultDto
    {
        /// <summary>
        /// 是否导入成功（错误处理为「跳过」时，即使有失败行也返回true）
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 成功导入行数
        /// </summary>
        public int SuccessCount { get; set; }

        /// <summary>
        /// 失败行数
        /// </summary>
        public int FailCount { get; set; }

        /// <summary>
        /// 错误信息列表（包含失败行号+原因）
        /// </summary>
        public List<string> ErrorMessages { get; set; } = new List<string>();
    }
}
