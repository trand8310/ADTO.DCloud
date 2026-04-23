using ADTO.DCloud.DatabaseManager;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.DatabaseManager.Dto
{
    /// <summary>
    /// 数据链接管理表
    /// </summary>
    [AutoMap(typeof(DataConnections))]
    public class DataConnectionDto : FullAuditedEntityDto<Guid>
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
        public Guid? TenantId { get; set; }
    }
}
