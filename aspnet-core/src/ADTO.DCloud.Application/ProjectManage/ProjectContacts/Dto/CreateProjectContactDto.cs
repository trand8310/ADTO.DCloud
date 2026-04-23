using System;
using ADTO.DCloud.Project;
using ADTOSharp.AutoMapper;
using ADTOSharp.Application.Services.Dto;

namespace ADTO.DCloud.ProjectManage.Dto
{
    /// <summary>
    /// 新增客户联系人
    /// </summary>
    [AutoMapTo(typeof(ProjectContacts))]
    public class CreateProjectContactDto : EntityDto<Guid?>
    {
        /// <summary>
        /// 项目Id
        /// </summary>
        public Guid ProjectId { get; set; }

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
    }
}
