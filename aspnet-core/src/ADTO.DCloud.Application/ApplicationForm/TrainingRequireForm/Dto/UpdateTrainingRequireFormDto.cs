using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using ADTOSharp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.ApplicationForm.TrainingRequireForm.Dto
{
    [AutoMapTo(typeof(Adto_TrainingRequireForm))]
    public class UpdateTrainingRequireFormDto : FullAuditedEntityDto<Guid>, IRemark
    {

        /// <summary>
        /// 用户Id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 公司Id
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// 部门Id
        /// </summary>
        public Guid DepartmentId { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [StringLength(225)]
        public string Title { get; set; }

        /// 开始日期
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 申请原因
        /// </summary>
        [StringLength(500)]
        public string Remark { get; set; }
    }
}
