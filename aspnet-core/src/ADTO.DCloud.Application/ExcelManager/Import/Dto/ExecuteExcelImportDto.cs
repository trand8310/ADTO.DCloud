using System;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ADTO.DCloud.ExcelManager.Import.Dto
{
    /// <summary>
    /// 导入Excel 
    /// </summary>
    public class ExecuteExcelImportDto
    {
        /// <summary>
        /// 导入配置ID（关联ExcelImport表的Code）
        /// </summary>
        [Required(ErrorMessage = "导入配置ID不能为空")]
        public string ImportCode { get; set; }

        ///// <summary>
        ///// 上传的Excel文件
        ///// </summary>
        //[Required(ErrorMessage = "请上传Excel文件")]
        //public IFormFile File { get; set; }

        /// <summary>
        /// Excel文件内容（字节数组）
        /// </summary>
        [Required(ErrorMessage = "请上传Excel文件")]
        public byte[] FileContent { get; set; }

        /// <summary>
        /// 原始文件名（用于日志和格式验证）
        /// </summary>
        public string FileName { get; set; }
    }
}
