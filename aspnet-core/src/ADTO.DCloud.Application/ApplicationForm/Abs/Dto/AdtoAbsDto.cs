using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.ApplicationForm.Abs.Dto
{

    [AutoMap(typeof(Adto_Abs))]
    public class AdtoAbsDto : FullAuditedEntityDto<Guid>
    {

        /// <summary>
        /// 用户Id
        /// </summary>
        [Display(Name = "用户Id", Description = "用户Id")]
        public Guid UserId { get; set; }

        /// <summary>
        /// 公司Id
        /// </summary>
         [Display(Description = "公司Id")]
        public Guid CompanyId { get; set; }

        /// <summary>
        /// 部门Id
        /// </summary>
        [Description("部门Id")]
        public Guid DepartmentId { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [Display(Name = "标题", Description = "标题")]
        [StringLength(225)]
        public string Title { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        [Description("开始日期")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        [Description("结束日期")]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 请假类型
        /// </summary>
        [StringLength(100)]
        [Description("请假类型")]
        public string AbsType { get; set; }

        /// <summary>
        /// 请假天数
        /// </summary>
        [Description("请假天数")]
        public decimal Days { get; set; }

        /// <summary>
        /// 请假原因
        /// </summary>
        [StringLength(500)]
        [Description("请假原因")]
        public string Remark { get; set; }

        /// <summary>
        /// 结婚证附件
        /// </summary>
        [StringLength(500)]
        [Description("结婚证附件")]
        public string MarriageAttach { get; set; }

        /// <summary>
        /// 调休备注
        /// </summary>
        [StringLength(500)]
        [Description("调休备注")]
        public string RestSummary { get; set; }

        #region 扩展字段
        /// <summary>
        /// 工号
        /// </summary>
        [Description("工号")]
        public string UserName { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        [Description("姓名")]
        public string Name { get; set; }
        /// <summary>
        /// 所属公司
        /// </summary>
        [Description("所属公司")]
        public string CompanyName { get; set; }
        /// <summary>
        /// 所属部门
        /// </summary>
        [Description("所属部门")]
        public string DepartmentName { get; set; }
        /// <summary>
        /// 请假类型
        /// </summary>
        [Description("请假类型")]
        public string AbsTypeName { get; set; }

        /// <summary>
        /// 流程进程是否结束1是0不是
        /// </summary>
        [Description("流程状态")]
        public int IsFinished { get; set; }
        /// <summary>
        /// 流程编码
        /// </summary>
        [Description("流程编码")]
        public string SchemeCode { get; set; }
        #endregion
    }
}
