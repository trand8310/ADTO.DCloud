using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.ApplicationForm.Att.Dto
{

    [AutoMapTo(typeof(Adto_Att))]
    public class UpdateAdtoAttDto:EntityDto<Guid>
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

        /// <summary>
        /// 异常时间
        /// </summary>
        public DateTime AttDate { get; set; }

        /// <summary>
        /// 异常类型
        /// </summary>
        [StringLength(100)]
        public string AttType { get; set; }

        /// <summary>
        /// 异常原因
        /// </summary>
        [StringLength(500)]
        public string Remark { get; set; }

        /// <summary>
        /// 附件证明
        /// </summary>
        public string Attachment { get; set; }
    }
}
