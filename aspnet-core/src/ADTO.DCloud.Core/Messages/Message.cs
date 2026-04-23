using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Messages
{
    /// <summary>
    /// 即时通讯消息内容
    /// </summary>
    [Table("Messages")]
    public class Message : Entity<Guid>, ICreationAudited, IDeletionAudited
    {

        #region 实体成员
        /// <summary>
        /// 发送者ID
        /// </summary>
        public string SendUserId { get; set; }
        /// <summary>
        /// 接收者ID
        /// </summary>
        public string RecvUserId { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 是否是系统消息
        /// </summary>
        public int? IsSystem { get; set; }
        /// <summary>
        /// 消息是否已读 1 是 0 不是；系统消息起作用
        /// </summary>
        public int? IsRead { get; set; }

        /// <summary>
        /// 消息内容id
        /// </summary>
        public string ContentId { get; set; }
        #endregion


        #region 多租户
        /// <summary>
        /// 租户ID
        /// </summary>
        public string TenantId { get; set; }
        public Guid? DeleterUserId { get; set; }
        public DateTime? DeletionTime { get; set; }
        public bool IsDeleted { get; set; }
        public Guid? CreatorUserId { get; set; }
        public DateTime CreationTime { get; set; }
        #endregion
    }
}
