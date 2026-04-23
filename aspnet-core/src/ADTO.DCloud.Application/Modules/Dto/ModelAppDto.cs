using ADTOSharp.Application.Services.Dto;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Modules.Dto
{
    /// <summary>
    /// 手机端菜单显示字段
    /// </summary>
    [AutoMap(typeof(Module))]
    public class ModelAppDto : EntityDto<Guid>
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
        /// App 图标
        /// </summary>
        public string AppIcon { get; set; }

        /// <summary>
        /// App 页面路径
        /// </summary>
        public string AppUrlPath { get; set; }
    }
}
