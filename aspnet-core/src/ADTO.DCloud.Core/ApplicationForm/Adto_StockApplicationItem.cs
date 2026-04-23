using ADTOSharp.Domain.Entities;
using ADTOSharp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.ApplicationForm
{
    /// <summary>
    /// 物品申请明细表包括（电脑、手机、总裁办工装及总裁办其他办公用品申请）
    /// </summary>
    [Table("Adto_StockApplicationItems")]
    public class Adto_StockApplicationItem : FullAuditedEntity<Guid>, IRemark
    {
        /// <summary>
        /// 所属申请单
        /// </summary>
        public Adto_StockApplication StockApplication { get; set; }
        public virtual Guid? StockApplicationId { get; set; }

        /// <summary>
        //  产品分类
        /// </summary>
        [StringLength(128)]
        public Guid? Category { get; set; }

        /// <summary>
        /// 申请类型
        /// </summary>
        [StringLength(100)]
        public string ApplyType { get; set; }

        /// <summary>
        /// 产品Id
        /// </summary>
        public Guid? ProductId { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        [StringLength(225)]
        public string ProductName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string Remark { get; set; }

        /// <summary>
        /// 归还时间
        /// </summary>
        public DateTime? BackTime { get; set; }

        /// <summary>
        /// 归还操作用户
        /// </summary>
        [StringLength(128)]
        public string BackUser { get; set; }

        /// <summary>
        /// 归还操作用户
        /// </summary>
        [StringLength(128)]
        public Guid BackUserId { get; set; }

        /// <summary>
        /// 归还备注
        /// </summary>
        [StringLength(500)]
        public string BackRemark { get; set; }

        /// <summary>
        /// 归还状态
        /// </summary>
        public bool ReturnStatus { get; set; }

        /// <summary>
        /// 申请数量
        /// </summary>
        public int ApplyCount { get; set; }
    }
}
