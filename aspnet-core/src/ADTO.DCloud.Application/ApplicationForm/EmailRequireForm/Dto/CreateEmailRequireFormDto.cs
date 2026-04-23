using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.ApplicationForm.EmailRequireForm.Dto
{
    [AutoMapTo(typeof(Adto_EmailRequireForm))]
    public class CreateEmailRequireFormDto:FullAuditedEntityDto<Guid>
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
        /// 邮箱账号
        /// </summary>
        [StringLength(100)]
        public string EmailName { get; set; }

        /// <summary>
        /// 邮箱密码
        /// </summary>
        [StringLength(100)]
        public string EmailPwd { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string Remark { get; set; }
    }
}
