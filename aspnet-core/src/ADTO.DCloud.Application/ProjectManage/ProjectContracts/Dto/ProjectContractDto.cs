using ADTO.DCloud.Project;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace ADTO.DCloud.ProjectManage.Dto
{
    /// <summary>
    /// 项目合同
    /// </summary>
    [AutoMap(typeof(ProjectContract))]
    public class ProjectContractDto : EntityDto<Guid>
    {
        public ProjectContractDto()
        {
            UploadFilesDtos = new List<ProjectUploadFilesDto>();
        }

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
        public string Code { get; set; }

        /// <summary>
        /// 签约时间
        /// </summary>
        public DateTime SigningTime { get; set; }

        /// <summary>
        /// 合同金额
        /// </summary>
        public decimal ContractAmount { get; set; }

        /// <summary>
        /// 币种（字典）
        /// </summary>
        public string CurrencyType { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 签约地址
        /// </summary>     
        public string SigningAt { get; set; }

        /// <summary>
        /// 合同内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 附件夹Id
        /// </summary>
        public Guid? FolderId { get; set; }

        /// <summary>
        /// 甲方名称
        /// </summary>
        public string PartyAName { get; set; }

        /// <summary>
        /// 甲方签约人
        /// </summary>
        public string PartyASignatory { get; set; }

        /// <summary>
        /// 甲方联系人
        /// </summary>
        public string PartyAContacts { get; set; }

        /// <summary>
        /// 甲方联系电话
        /// </summary>
        public string PartyAPhone { get; set; }

        /// <summary>
        /// 甲方开户行
        /// </summary>
        public string PartyABank { get; set; }

        /// <summary>
        /// 甲方银行账号
        /// </summary>
        public string PartyABankAccount { get; set; }

        /// <summary>
        /// 乙方名称
        /// </summary>
        public string PartyBName { get; set; }

        /// <summary>
        ///  乙方签约人
        /// </summary>
        public string PartyBSignatory { get; set; }

        /// <summary>
        /// 乙方联系人
        /// </summary>
        public string PartyBContacts { get; set; }

        /// <summary>
        /// 乙方联系电话
        /// </summary>
        public string PartyBPhone { get; set; }

        /// <summary>
        /// 乙方开户行
        /// </summary>
        public string PartyBBank { get; set; }

        /// <summary>
        /// 乙方银行账号
        /// </summary>
        public string PartyBBankAccount { get; set; }

        /// <summary>
        /// 审核状态
        /// </summary>
        public int AuditStatus { get; set; }

        /// <summary>
        /// 添加人
        /// </summary>
        public string CreatorUserName { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public Guid CreatorUserId { get; set; }

        /// <summary>
        /// 图片集合
        /// </summary>
        public List<ProjectUploadFilesDto> UploadFilesDtos { get; set; }
    }
}
