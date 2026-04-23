using System;
using System.ComponentModel;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADTO.DCloud.ExcelManager
{
    /// <summary>
    /// Excel 导出配置主表
    /// </summary>
    [Description("Excel导出配置主表"), Table("ExcelExports")]
    public class ExcelExport : FullAuditedAggregateRoot<Guid>, IMayHaveTenant, IPassivable, IRemark
    {
        /// <summary>
        /// 配置名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 唯一编码
        /// </summary>
        public string Code { get; set; }
       
        /// <summary>
        /// 业务服务完整名
        /// 例：ADTO.DCloud.IUserAppService
        /// </summary>
        public string ServiceFullName { get; set; }

        /// <summary>
        /// 调用方法名
        /// 例：GetAllListAsync
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        [Description("是否有效")]
        public bool IsActive { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Description("备注")]
        public string Remark { get; set; }

        /// <summary>
        /// 租户
        /// </summary>
        [Description("租户")]
        public Guid? TenantId { get; set; }
    }
}
