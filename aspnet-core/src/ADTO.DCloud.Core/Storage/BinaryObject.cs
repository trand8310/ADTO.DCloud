using ADTOSharp;
using ADTOSharp.Domain.Entities;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADTO.DCloud.Storage
{
    /// <summary>
    /// 二进制文件对像
    /// </summary>
    [Table("BinaryObjects"),Description("文件对像")]
    public class BinaryObject : Entity<Guid>, IMayHaveTenant
    {
        /// <summary>
        /// 租户Id
        /// </summary>
        [Description("租户Id")]
        public virtual Guid? TenantId { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [Description("描述")]
        public virtual string Description { get; set; }

        /// <summary>
        /// 文件的二进制内容
        /// </summary>
        [Required,Description("内容"), MaxLength(BinaryObjectConsts.BytesMaxSize)]
        public virtual byte[] Bytes { get; set; }

        public BinaryObject()
        {
            Id = SequentialGuidGenerator.Instance.Create();
        }

        public BinaryObject(Guid? tenantId, byte[] bytes, string description = null)
            : this()
        {
            TenantId = tenantId;
            Bytes = bytes;
            Description = description;
        }
    }
}
