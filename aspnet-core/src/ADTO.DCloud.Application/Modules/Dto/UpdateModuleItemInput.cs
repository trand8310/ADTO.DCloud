using ADTO.DCloud.Authorization.Users.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Domain.Entities.Auditing;
using ADTOSharp.Domain.Entities;
using System;
using System.Collections.Generic;
using ADTO.DCloud.Authorization.Users;
using ADTOSharp.AutoMapper;


namespace ADTO.DCloud.Modules.Dto
{
    [AutoMapTo(typeof(ModuleItem))]
    public class UpdateModuleItemInput : EntityDto<Guid>
    {

        /// <summary>
        /// 名称
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// HtmlDomId
        /// </summary>
        public string HtmlDomId { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Css类型
        /// </summary>
        public string CssClass { get; set; }

        /// <summary>
        /// 元素调用脚本
        /// </summary>
        public string JsScript { get; set; }
        /// <summary>
        /// 排序代码
        /// </summary>
        public int DisplayOrder { get; set; }
        /// <summary>
        /// 权限标识
        /// </summary>
        public string Permission { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        public Guid ModuleId { get; set; }

    }
}
