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
    [AutoMapFrom(typeof(Module))]
    public class ModuleListDto : EntityDto<Guid>, IHasCreationTime
    {
        /// <summary>
        /// 模块名称
        /// </summary>
        public string ModuleName { get; set; }
        /// <summary>
        /// 模块标题
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 页面Path
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 页面路径,Component
        /// </summary>
        public string Component { get; set; }
        /// <summary>
        /// 权限标识
        /// </summary>
        public string Permission { get; set; }


        /// <summary>
        /// 动态权限标识,1行一个权限标识
        /// </summary>
        public string DynamicPermission { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Css类型
        /// </summary>
        public string CssClass { get; set; }
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

        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 菜单类别,0:菜单,1:目录,2:按钮,3:外链
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
