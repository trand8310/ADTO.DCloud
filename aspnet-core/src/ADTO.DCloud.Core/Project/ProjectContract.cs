using System;
using System.ComponentModel;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADTO.DCloud.Project
{
    /// <summary>
    /// 项目合同表
    /// </summary>
    [Description("项目合同表"), Table("ProjectContracts")]
    public class ProjectContract : FullAuditedAggregateRoot<Guid>, IRemark, IMayHaveTenant
    {
        /// <summary>
        /// 所属项目
        /// </summary>
        [ForeignKey("ProjectInfo")]
        [Description("所属项目Id")]
        public Guid ProjectId { get; set; }
        public virtual ProjectInfo ProjectInfo { get; set; }

        /// <summary>
        /// 合同编号
        /// </summary>
        [Description("合同编号")]
        public string Code { get; set; }

        /// <summary>
        /// 签约时间
        /// </summary>
        [Description("签约时间")]
        public DateTime SigningTime { get; set; }

        /// <summary>
        /// 合同金额
        /// </summary>
        [Description("合同金额")]
        public decimal ContractAmount { get; set; }

        /// <summary>
        /// 币种（字典）
        /// </summary>
        [Description("币种")]
        public string CurrencyType { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [Description("开始时间")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [Description("结束时间")]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 签约地址
        /// </summary>    
        [Description("签约地址")]
        public string SigningAt { get; set; }

        /// <summary>
        /// 合同内容
        /// </summary>
        [Description("合同内容")]
        public string Content { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 租户Id
        /// </summary>
        [Description("租户Id")]
        public Guid? TenantId { get; set; }

        /// <summary>
        /// 附件夹Id
        /// </summary>
        public Guid? FolderId { get; set; }

        /// <summary>
        /// 甲方名称
        /// </summary>
        [Description("甲方名称")]
        public string PartyAName { get; set; }

        /// <summary>
        /// 甲方签约人
        /// </summary>
        [Description("甲方签约人")]
        public string PartyASignatory { get; set; }

        /// <summary>
        /// 甲方联系人
        /// </summary>
        [Description("甲方联系人")]
        public string PartyAContacts { get; set; }

        /// <summary>
        /// 甲方联系电话
        /// </summary>
        [Description("甲方联系电话")]
        public string PartyAPhone { get; set; }

        /// <summary>
        /// 甲方开户行
        /// </summary>
        [Description("甲方开户行")]
        public string PartyABank { get; set; }

        /// <summary>
        /// 甲方银行账号
        /// </summary>
        [Description("甲方银行账号")]
        public string PartyABankAccount { get; set; }

        /// <summary>
        /// 乙方名称
        /// </summary>
        [Description("乙方名称")]
        public string PartyBName { get; set; }

        /// <summary>
        ///  乙方签约人
        /// </summary>
        [Description("乙方签约人")]
        public string PartyBSignatory { get; set; }

        /// <summary>
        /// 乙方联系人
        /// </summary>
        [Description("乙方联系人")]
        public string PartyBContacts { get; set; }

        /// <summary>
        /// 乙方联系电话
        /// </summary>
        [Description("乙方联系电话")]
        public string PartyBPhone { get; set; }

        /// <summary>
        /// 乙方开户行
        /// </summary>
        [Description("乙方开户行")]
        public string PartyBBank { get; set; }

        /// <summary>
        /// 乙方银行账号
        /// </summary>
        [Description("乙方银行账号")]
        public string PartyBBankAccount { get; set; }

        /// <summary>
        /// 审核状态
        /// </summary>
        [Description("审核状态")]
        public int? AuditStatus { get; set; }
    }
}
