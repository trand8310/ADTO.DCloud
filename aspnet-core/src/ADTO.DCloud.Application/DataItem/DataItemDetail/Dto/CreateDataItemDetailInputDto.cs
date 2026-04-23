using System;
using System.ComponentModel.DataAnnotations;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;

namespace ADTO.DCloud.DataItem.Dto
{
    /// <summary>
    /// 添加字典详情
    /// </summary>
    [AutoMapTo(typeof(DataItemDetail))]
    public class CreateDataItemDetailInputDto : EntityDto<Guid>
    {
        /// <summary>
        /// 所属分类编码（前端是传的分类编码，保存时要根据编码查分类Id）
        /// </summary>
        public string ItemCode { get; set; }

        /// <summary>
        /// 分类主键
        /// </summary>
        public Guid ItemId { get; set; }

        /// <summary>
        /// 父级主键
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public string ItemValue { get; set; }

        /// <summary>
        /// 租户
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// 排序值
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
