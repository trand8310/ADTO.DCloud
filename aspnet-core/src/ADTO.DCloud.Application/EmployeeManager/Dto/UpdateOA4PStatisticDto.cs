using ADTOSharp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;



namespace ADTO.DCloud.EmployeeManager.Dto
{
    /// <summary>
    /// 
    /// </summary>
    public class UpdateOA4PStatisticDto : EntityDto<Guid>
    {

        /// <summary>
        /// 是否参与面试
        /// </summary>
        public int? IsInterviewed { get; set; }
        /// <summary>
        /// 面试评价
        /// </summary>
        [StringLength(500)]
        public string InterviewRating { get; set; }
        /// <summary>
        /// 是否通过
        /// </summary>
        public int? IsPassed { get; set; }
        /// <summary>
        /// 拟入职日期
        /// </summary>
        public DateTime? ExpectedJoiningDate { get; set; }

        /// <summary>
        /// 电子简历附件
        /// </summary>
        [StringLength(128)]
        public string ResumeFile { get; set; }

        /// <summary>
        /// 是否Offer
        /// </summary>
        public int? IsOffer { get; set; }
    }
}
