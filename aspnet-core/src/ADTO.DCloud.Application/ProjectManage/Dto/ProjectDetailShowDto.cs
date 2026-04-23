using ADTO.DCloud.Customers.CustomerContactManage.Dto;
using ADTO.DCloud.Customers.Dto;
using ADTO.DCloud.Project;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ADTO.DCloud.ProjectManage.Dto
{
    /// <summary>
    /// 项目详情
    /// </summary>
    [AutoMap(typeof(ProjectInfo))]
    public class ProjectDetailShowDto : EntityDto<Guid>
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
        /// 联系人
        /// </summary>
        public List<ProjectContactsDto> ProjectContacts { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public Guid CreatorUserId { get; set; }

        /// <summary>
        /// 创建人名称
        /// </summary>
        public string CreatorUserName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 附件夹Id
        /// </summary>
        public Guid? FolderId { get; set; }

        /// <summary>
        /// 图片集合
        /// </summary>
        public List<ProjectUploadFilesDto> UploadFilesDtos { get; set; }
    }
}
