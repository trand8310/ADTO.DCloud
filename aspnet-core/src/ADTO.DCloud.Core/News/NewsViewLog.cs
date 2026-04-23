using ADTOSharp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.News
{
    /// <summary>
    /// 新闻浏览记录
    /// </summary>
    public class NewsViewLog : CreationAuditedEntity<Guid>
    {
        /// <summary>
        /// 新闻Id
        /// </summary>
        public Guid NewsId { get; set; }

        /// <summary>
        /// 浏览人
        /// </summary>
        public string UserName { get; set; }
    }
}
