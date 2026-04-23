
using ADTOSharp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADTO.DCloud.Storage
{
    /// <summary>
    /// 文件共享权限-根据文件类别设置共享文件权限
    /// </summary>
    [Table("SharedFileAuthorizes")]
    public class SharedFileAuthorizes : CreationAuditedEntity<Guid>
    {
        /// <summary>
        /// 共享类别Id
        /// </summary>
        public Guid SharedFileCategory { get; set; }

        /// <summary>
        /// 共享权限对象Id
        /// </summary>
        public Guid ObjectId { get; set; }

        /// <summary>
        /// 共享权限对象名称
        /// </summary>
        public string ObjectName { get; set; }

        /// <summary>
        /// 共享权限类型（公司，部门，用户），暂时默认部门
        /// </summary>
        public string ObjectType { get; set; }


    }
}
