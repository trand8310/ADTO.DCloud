using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using ADTOSharp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.ApplicationForm.MerchandiseApplication.Dto
{
    [AutoMapTo(typeof(Adto_OfficeSupplyApplication))]
    public class UpdateOfficeSupplyApplicationDto : FullAuditedEntityDto<Guid>, IRemark
    {
        /// <summary>
        /// 标题
        /// </summary>
        [StringLength(225)]
        public string Title { get; set; }

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
        /// 申请理由
        /// </summary>
        [StringLength(500)]
        public string Remark { get; set; }
    }
}
