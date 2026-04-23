using System;
using ADTO.DCloud.Project;
using System.ComponentModel;
using ADTOSharp.AutoMapper;
using ADTOSharp.Application.Services.Dto;

namespace ADTO.DCloud.ProjectManage.Dto
{
    /// <summary>
    /// 项目基本信息
    /// </summary>
    [AutoMap(typeof(ProjectInfo))]
    public class ProjectInfoDto : EntityDto<Guid>
    {
        /// <summary>
        /// 项目名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 项目编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 项目规模
        /// </summary>
        public decimal Scale { get; set; }

        /// <summary>
        /// 项目金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 是否激活
        /// </summary>
        [Description("是否激活")]
        public bool IsActive { get; set; }

        /// <summary>
        /// 项目地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Description("排序")]
        public int DisplayOrder { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public Guid CreatorUserId { get; set; }

        /// <summary>
        /// 添加人
        /// </summary>
        public string CreatorUserName { get; set; }

        /// <summary>
        /// 是否签订合同
        /// </summary>
        public bool IsSign { get; set; }

        /// <summary>
        /// 联系人电话（第一条）
        /// </summary>
        public string FirstContactPhone { get; set; }

        /// <summary>
        /// 附件夹Id
        /// </summary>
        public Guid? FolderId { get; set; }
    }
}
