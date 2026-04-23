
using ADTOSharp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;


namespace ADTO.DCloud.EmployeeManager.Dto
{
    /// <summary>
    /// 上传4P测试电子简历
    /// </summary>
    public class Upload4PTestDto:EntityDto<Guid>
    {
        /// <summary>
        /// 电子简历附件
        /// </summary>
        [StringLength(128)]
        public string ResumeFile { get; set; }
    }
}
