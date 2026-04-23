using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.ApplicationForm.Att.Dto
{
    /// <summary>
    /// 考勤异常Dto
    /// </summary>
    [AutoMap(typeof(Adto_Att))]
    public class AdtoAttDto:FullAuditedEntityDto<Guid>
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        [JsonIgnore]
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
        /// 岗位Id
        /// </summary>
        public int? PostId { get; set; }

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
        /// 异常类型
        /// </summary>
        public string AttTypeName { get; set; }

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
