using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using ADTOSharp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.ApplicationForm.StockApplication.Dto
{
    /// <summary>
    /// 库存申请明细Dto
    /// </summary>
    [AutoMap(typeof(Adto_StockApplicationItem))]
    public class StockApplicationItemDto : FullAuditedEntityDto<Guid>, IRemark
    {
        /// <summary>
        /// 所属申请单
        /// </summary>
        public Adto_StockApplication StockApplication { get; set; }
        public  Guid StockApplicationId { get; set; }

        /// <summary>
        //  产品分类
        /// </summary>
        [StringLength(128)]
        public string Category { get; set; }

        /// <summary>
        /// 申请类型
        /// </summary>
        [StringLength(100)]
        public string ApplyType { get; set; }

        /// <summary>
        /// 产品Id
        /// </summary>
        [StringLength(128)]
        public string ProductId { get; set; }

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
    }
}
