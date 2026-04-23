using System;
using ADTO.DCloud.Project;
using System.Collections.Generic;
using System.ComponentModel;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;

namespace ADTO.DCloud.ProjectManage.Dto
{
    /// <summary>
    /// 新增项目跟进记录实体
    /// </summary>
    [AutoMapTo(typeof(ProjectFollowRecord))]
    public class CreateProjectFollowRecordDto : EntityDto<Guid?>
    {
        public CreateProjectFollowRecordDto() { UploadFilesDtos = new List<ProjectUploadFilesDto>(); }

        /// <summary>
        /// 所属项目
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        /// 跟进方式 字典
        /// </summary>
        public string ProjectFollowType { get; set; }

        /// <summary>
        /// 跟进阶段 字段
        /// </summary>
        public string ProjectFollowStage { get; set; }

        /// <summary>
        /// 跟进时间
        /// </summary>
        public DateTime FollowTime { get; set; }

        /// <summary>
        /// 跟进内容
        /// </summary>
        [Description("跟进内容")]
        public string Content { get; set; }

        /// <summary>
        /// 跟进人Id
        /// </summary>
        public Guid FollowUserId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

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
