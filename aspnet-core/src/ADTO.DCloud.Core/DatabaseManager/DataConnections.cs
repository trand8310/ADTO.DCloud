using System;
using System.ComponentModel;
using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADTO.DCloud.DatabaseManager
{
    /// <summary>
    /// 数据链接管理表
    /// </summary>
    [Description("数据链接管理表"), Table("DataConnections")]
    public class DataConnections : FullAuditedEntity<Guid>, IMayHaveTenant
    {
        /// <summary>
        /// 数据库名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 数据库链接地址
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// 数据库名称
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// 数据库链接账号
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 数据库链接密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 租户ID
        /// </summary>
        [Description("表外键租户IDId")]
        public Guid? TenantId { get; set; }

    }
}
