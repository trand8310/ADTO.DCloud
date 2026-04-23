using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.EmployeeManager.Dto
{
    /// <summary>
    /// 员工变更记录
    /// </summary>
    [AutoMap(typeof(EmployeeChangeLog))]
    public class EmployeeChangeLogDto
    {
        /// <summary>
        /// 对象表Id
        /// </summary>
        public Guid Objectid { get; set; }

        /// <summary>
        /// 表
        /// </summary>
        public string ObjectName { get; set; }
        /// <summary>
        /// 字段名
        /// </summary>
        public string PropertyName { get; set; }
        /// <summary>
        /// 旧值
        /// </summary>
        public string OldValue { get; set; }
        /// <summary>
        /// 新值
        /// </summary>
        public string NewValue { get; set; }
    }
}
