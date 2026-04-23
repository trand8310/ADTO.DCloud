using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Processs.Dto
{
    /// <summary>
    /// 流程条件
    /// </summary>
    public class WorkFlowCondition
    {
        /// <summary>
        /// 流程条件编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 条件名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 类型 1比较字段 2 sql语句
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 数据库
        /// </summary>
        public string DbCode { get; set; }
        /// <summary>
        /// 数据库表
        /// </summary>
        public string Table { get; set; }
        /// <summary>
        /// 流程关联字段
        /// </summary>
        public string Rfield { get; set; }
        /// <summary>
        /// 比较字段
        /// </summary>
        public string Cfield { get; set; }
        /// <summary>
        /// 比较类型 1等于 2不等于 3大于 4大于等于 5小于 6小于等于 7包含 8不包含 9包含于 10不包含于
        /// </summary>
        public string CompareType { get; set; }
        /// <summary>
        /// 比较值
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// sql语句
        /// </summary>
        public string Sql { get; set; }



    }
}
