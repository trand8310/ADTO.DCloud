using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.FormScheme.Model
{
    /// <summary>
    /// 表单组件
    /// </summary>
    public class FormComponentModel
    {
        /// <summary>
        /// 组件类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 文本
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// 标识
        /// </summary>
        public string Prop { get; set; }
        /// <summary>
        /// 绑定表
        /// </summary>
        public string Table { get; set; }
        /// <summary>
        /// 绑定字段
        /// </summary>
        public string Field { get; set; }
        /// <summary>
        /// 单据编码
        /// </summary>
        public string Code { get; set; }
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
        public bool isDESC { get; set; }
        /// <summary>
        /// 数据字段类型
        /// </summary>
        public string CsType { get; set; }

        /// <summary>
        /// 子组件列表
        /// </summary>
        public List<FormComponentModel> children { get; set; }


        /// <summary>
        /// 字段长度
        /// </summary>
        public int? FieldLong { get; set; }
        /// <summary>
        /// 字段类型
        /// </summary>
        public string FieldFormat { get; set; }
        /// <summary>
        /// 表备注
        /// </summary>
        public string TableNotes { get; set; }
    }
}
