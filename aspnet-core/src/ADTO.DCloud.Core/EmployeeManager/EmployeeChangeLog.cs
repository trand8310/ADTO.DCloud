using ADTOSharp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.EmployeeManager
{
    /// <summary>
    /// 员工变更记录
    /// </summary>
    [Description("员工变更记录"), Table("EmployeeChangeLogs")]
    public class EmployeeChangeLog:CreationAuditedEntity<Guid>
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
