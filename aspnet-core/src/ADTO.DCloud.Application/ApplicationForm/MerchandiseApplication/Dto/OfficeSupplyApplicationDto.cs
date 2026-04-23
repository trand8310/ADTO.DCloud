using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using ADTOSharp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ADTO.DCloud.ApplicationForm.MerchandiseApplication.Dto
{

    [AutoMap(typeof(Adto_OfficeSupplyApplication))]
    public class OfficeSupplyApplicationDto : FullAuditedEntityDto<Guid>, IRemark
    {
        /// <summary>
        /// 标题
        /// </summary>
        [StringLength(225),JsonIgnore]
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
        #region 扩展字段
        /// <summary>
        /// 工号
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 所属公司
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 所属部门
        /// </summary>
        public string DepartmentName { get; set; }
        /// <summary>
        /// 请假类型
        /// </summary>
        public string AbsTypeName { get; set; }

        /// <summary>
        /// 流程进程是否结束1是0不是
        /// </summary>
        public int IsFinished { get; set; }
        /// <summary>
        /// 流程编码
        /// </summary>
        public string SchemeCode { get; set; }
        #endregion
    }
}
