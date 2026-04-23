using System;
using System.ComponentModel;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ADTO.DCloud.FormScheme
{
    /// <summary>
    /// 表单模板信息（主表）
    /// </summary>
    [Description("表单模板信息（主表）"), Table("FormSchemeInfos")]
    public class FormSchemeInfo : FullAuditedEntity<Guid>, IMayHaveTenant, IPassivable, IRemark
    {
        /// <summary>
        /// 表单名字
        /// </summary>
        [Description("表单名字")]
        public string Name { get; set; }

        /// <summary>
        /// 表单分类
        /// </summary>
        [Description("表单分类")]
        public string Category { get; set; }

        /// <summary>
        /// 当前模板主键
        /// </summary>
        [Description("当前模板主键")]
        public Guid SchemeId { get; set; }

        /// <summary>
        /// 表单类型 1 视图表单 0其他数据库表单 2自动建表
        /// </summary>
        [Description("表单类型 1 视图表单 0其他数据库表单 2自动建表")]
        public int? FormType { get; set; }

        /// <summary>
        /// 租户Id
        /// </summary>
        [Description("租户Id")]

        public Guid? TenantId { get; set; }

        /// <summary>
        /// 有效标志
        /// </summary>
        [Description("有效标志")]
        public bool IsActive { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Description("备注")]
        public string Remark { get; set; }

        /// <summary>
        /// 自定义服务方法-通过对应的服务方法来保存表单信息
        /// </summary>
        [StringLength(125)]
        public string MethodName { get; set; }
    }
}
