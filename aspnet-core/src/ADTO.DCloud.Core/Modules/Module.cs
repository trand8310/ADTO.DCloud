using ADTOSharp.Domain.Entities.Auditing;
using ADTOSharp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace ADTO.DCloud.Modules
{
    [Table("Modules")]
    public class Module : FullAuditedEntity<Guid>, IPassivable, IMayHaveTenant, IDisplayOrder, IRemark
    {
        [ForeignKey("ParentId")]
        public Guid? ParentId { get; set; }
        /// <summary>
        /// 父级
        /// </summary>
        public Module? Parent { get; set; }
        /// <summary>
        /// 模块名称
        /// </summary>
        public string ModuleName { get; set; }
        /// <summary>
        /// 模块标题
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 页面URL,Path
        /// </summary>
        public string Path { get; set; }
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
        /// 功能按钮
        /// </summary>
        public virtual ICollection<ModuleItem> ModuleItems { get; set; }
        /// <summary>
        /// 租户ID
        /// </summary>
        public virtual Guid? TenantId { get; set; }

        /// <summary>
        /// 页面路径,Component
        /// </summary>
        public string Component { get; set; }

        /// <summary>
        /// Redirect
        /// </summary>
        public string Redirect { get; set; }

        /// <summary>
        /// 菜单类别,0:菜单,1:目录,2:按钮,3:外链
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }


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
