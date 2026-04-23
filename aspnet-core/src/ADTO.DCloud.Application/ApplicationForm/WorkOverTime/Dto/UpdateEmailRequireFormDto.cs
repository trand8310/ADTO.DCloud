using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.ApplicationForm.WorkOverTime.Dto
{
    [AutoMapTo(typeof(Adto_WorkOverTime))]
    public class UpdateWorkOverTimeDto : AuditedEntityDto<Guid>
    {

        /// <summary>
        /// 用户Id
        /// </summary>
        public Guid UserID { get; set; }

        /// <summary>
        /// 公司Id
        /// </summary>
        public Guid? CompanyID { get; set; }

        /// <summary>
        /// 部门Id
        /// </summary>
        public Guid DepartmentId { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [StringLength(225)]
        public string Title { get; set; }
        /// <summary>
        /// 申请原因
        /// </summary>
        [StringLength(500)]
        public string Remark { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartDateTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDateTime { get; set; }

        /// <summary>
        /// 总计（小时） 
        /// 自行填写
        /// </summary>
        [StringLength(128)]
        public string Times { get; set; }
    }
}
