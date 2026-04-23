using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.FormScheme.Dto
{
    public class FormComponent
    {
        /// <summary>
        /// 组件ID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 组件容器ID
        /// </summary>
        public string ContainerId { get; set; }
        /// <summary>
        /// 组件名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 组件类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 组件配置
        /// </summary>
        public FormComponentConfig Config { get; set; }
    }

    public class FormComponentConfig
    {
        /// <summary>
        /// 组件名称
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// 绑定数据表
        /// </summary>
        public string Table { get; set; }

        /// <summary>
        /// 绑定字段
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// 数据字段类型
        /// </summary>
        public string CsType { get; set; }

        /// <summary>
        /// 排序字段
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        /// 排序字段类型
        /// </summary>
        public string OrderCsType { get; set; }
        /// <summary>
        /// 是否倒叙
        /// </summary>
        public bool IsDESC { get; set; }
    }
}
