using ADTO.DCloud.Project;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;
namespace ADTO.DCloud.ProjectManage.Dto
{
    /// <summary>
    /// 项目联系人
    /// </summary>
    [AutoMap(typeof(ProjectContacts))]
    public class ProjectContactsDto : EntityDto<Guid>
    {
        /// <summary>
        /// 项目Id
        /// </summary>
        public Guid ProjectId { get; set; }
        public virtual ProjectInfo ProjectInfo { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// 项目编码
        /// </summary>
        public string ProjectCode { get; set; }

        /// <summary>
        /// 联系人姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 联系人角色 字典
        /// </summary>
        public string ContactRole { get; set; }

        /// <summary>
        /// 职务
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 微信
        /// </summary>
        public string WeChat { get; set; }

        /// <summary>
        /// 其他账号
        /// </summary>
        public string OtherId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
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
    }
}
