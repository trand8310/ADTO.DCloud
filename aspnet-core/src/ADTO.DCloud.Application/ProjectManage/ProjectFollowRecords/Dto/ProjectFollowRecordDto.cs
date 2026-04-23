using System;
using ADTOSharp.AutoMapper;
using ADTOSharp.Application.Services.Dto;
using ADTO.DCloud.Project;
using System.Collections.Generic;

namespace ADTO.DCloud.ProjectManage.Dto
{
    /// <summary>
    /// 项目跟进记录
    /// </summary>
    [AutoMap(typeof(ProjectFollowRecord))]
    public class ProjectFollowRecordDto : EntityDto<Guid>
    {
        public ProjectFollowRecordDto()
        {
            UploadFilesDtos = new List<ProjectUploadFilesDto>();
        }

        /// <summary>
        /// 所属项目
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// 项目编码
        /// </summary>
        public string ProjectCode { get; set; }

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
        public string Content { get; set; }

        /// <summary>
        /// 跟进人Id
        /// </summary>
        public Guid FollowUserId { get; set; }

        public string FollowUserName { get; set; }

        /// <summary>
        /// 跟进人图像
        /// </summary>
        public string FollowProfilePicture { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 附件夹Id
        /// </summary>
        public Guid? FolderId { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public Guid CreatorUserId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 图片集合
        /// </summary>
        public List<ProjectUploadFilesDto> UploadFilesDtos { get; set; }

    }
}
