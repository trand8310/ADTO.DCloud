using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.DataAuthorizes.Model
{
    /// <summary>
    /// 设置条件
    /// </summary>
    public class ConditionModel
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int index { get; set; }
        /// <summary>
        /// 字段ID
        /// </summary>
        /// <returns></returns>
        public string FieldId { get; set; }
        /// <summary>
        /// 比较符1.等于2.大于3.大于等于.4小于.5小于等于 6.包含 7包含于 8不等于 9不包含 10不包含于
        /// </summary>
        /// <returns></returns>
        public int? Symbol { get; set; }
        /// <summary>
        /// 字段值类型1.文本2.登录者ID3.登录者账号4.登录者公司5.登录者部门6.登录者岗位7.登录者角色
        /// </summary>
        public int? FiledValueType { get; set; }
        /// <summary>
        /// 字段值
        /// </summary>
        /// <returns></returns>
        public string FiledValue { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        public Type FieldType { get; set; }

        /// <summary>
        /// 类型 glbd关联表单数据类型，需要特殊处理
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 表名称
        /// </summary>
        public string Table { get; set; }
        /// <summary>
        /// 比较字段
        /// </summary>
        public string Cfield { get; set; }
        /// <summary>
        /// 外键
        /// </summary>
        public string Field { get; set; }
        /// <summary>
        /// 关联表字段
        /// </summary>
        public string RelationField { get; set; }
        /// <summary>
        /// 分组编号
        /// </summary>
        public string Group { get; set; }
    }

    /// <summary>
    /// 分组公式
    /// </summary>
    public class GroupModel()
    {
        public string Value { get; set; }
    }

    public class ParseConditionModel()
    {
        public Dictionary<string, object> parameters { get; set; } = new Dictionary<string, object>();

        public string dynamicExpression { get; set; }
    }
}
