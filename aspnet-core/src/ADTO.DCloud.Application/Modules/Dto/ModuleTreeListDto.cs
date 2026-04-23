using ADTO.DCloud.Authorization.Users.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Domain.Entities.Auditing;
using ADTOSharp.Domain.Entities;
using System;
using System.Collections.Generic;
using ADTO.DCloud.Authorization.Users;
using ADTOSharp.AutoMapper;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace ADTO.DCloud.Modules.Dto
{
    [AutoMapFrom(typeof(ModuleDto))]
    public class ModuleTreeListDto : EntityDto<Guid>, IHasCreationTime, IPassivable, IDisplayOrder
    {
        public ModuleTreeListDto() { Children = new List<ModuleTreeListDto>(); }
        public Guid? ParentId { get; set; }
        /// <summary>
        /// 模块名称
        /// </summary>
        public string ModuleName { get; set; }
        /// <summary>
        /// 模块标题
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 页面,Path
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 权限标识
        /// </summary>
        public string Permission { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// Css类型
        /// </summary>
        public string CssClass { get; set; }
        /// <summary>
        /// 是否激活,true:正常,false:隐藏,不在导航列表中显示
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// 排序代码
        /// </summary>
        public int DisplayOrder { get; set; }
        /// <summary>
        /// 是否系统模块
        /// </summary>
        public bool IsSystem { get; set; }
        /// <summary>
        /// 前端界面是否缓存
        /// </summary>
        public bool? KeepAlive { get; set; }
        /// <summary>
        /// 是否叶子节点
        /// </summary>
        public bool IsLeaf { get; set; }
        /// <summary>
        /// 是否自动展开
        /// </summary>
        public bool IsAutoExpand { get; set; }

        /// <summary>
        /// 页面路径,Component
        /// </summary>
        public string Component { get; set; }
        /// <summary>
        /// Redirect
        /// </summary>
        public string Redirect { get; set; }

        public List<ModuleTreeListDto> Children { get; set; }

        public DateTime CreationTime { get; set; }
        /// <summary>
        /// 是否App显示
        /// </summary>
        public bool IsShowApp { get; set; }

        /// <summary>
        /// App 图标
        /// </summary>
        [StringLength(125)]
        public string AppIcon { get; set; }

        /// <summary>
        /// App 页面路径
        /// </summary>
        [StringLength(125)]
        public string AppUrlPath { get; set; }
    }
}
