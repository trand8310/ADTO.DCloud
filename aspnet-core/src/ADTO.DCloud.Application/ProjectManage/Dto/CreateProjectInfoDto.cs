using System;
using ADTO.DCloud.Project;
using ADTOSharp.AutoMapper;
using ADTOSharp.Application.Services.Dto;
using System.Collections.Generic;

namespace ADTO.DCloud.ProjectManage.Dto
{
    /// <summary>
    /// 添加项目
    /// </summary>
    [AutoMapTo(typeof(ProjectInfo))]
    public class CreateProjectInfoDto : EntityDto<Guid?>
    {
        public CreateProjectInfoDto()
        {
            UploadFilesDtos =
                new List<ProjectUploadFilesDto>();
        }
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
        public int DisplayOrder { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public List<CreateProjectContactDto> ProjectContacts { get; set; }

        /// <summary>
        /// 附件夹Id（手机端中有客户新增有附件）
        /// </summary>
        public Guid? FolderId { get; set; }

        /// <summary>
        /// 图片集合
        /// </summary>
        public List<ProjectUploadFilesDto> UploadFilesDtos { get; set; }
    }
}
