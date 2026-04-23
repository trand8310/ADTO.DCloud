using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Dto
{
    /// <summary>
    /// 数据权限过滤
    /// </summary>
    public class DataFilteredJoined<T>
    {
        /// <summary>
        /// 查询语句
        /// </summary>
        public IQueryable<T> Query { get; set; }
        /// <summary>
        /// 字段权限
        /// </summary>
        public string DataAuthFields { get; set; }
    }
}
