using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.ApplicationForm.InformalPetitions.Dto
{
    [AutoMap(typeof(Adto_InformalPetition))]
    public class InformalPetitionDto : FullAuditedEntityDto<Guid>
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
        /// 合同名称
        /// </summary>
        [StringLength(200)]
        public string ContractName { get; set; }
        /// <summary>
        /// 合同金额
        /// </summary>
        [StringLength(100)]
        public string ContractAmount { get; set; }
        /// <summary>
        /// 佣金
        /// </summary>
        [StringLength(100)]
        public string Commission { get; set; }
        /// <summary>
        /// 合同编号
        /// </summary>
        [StringLength(100)]
        public string ContractNo { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string Remark { get; set; }
        /// <summary>
        /// 附件
        /// </summary>
        [StringLength(500)]
        public string Files { get; set; }

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
