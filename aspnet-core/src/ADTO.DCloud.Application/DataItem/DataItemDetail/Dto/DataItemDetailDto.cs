using System;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using ADTOSharp.Domain.Entities.Auditing;

namespace ADTO.DCloud.DataItem.Dto
{
    /// <summary>
    /// 字典详情
    /// </summary>
    [AutoMap(typeof(DataItemDetail))]
    public class DataItemDetailDto : EntityDto<Guid>, IHasCreationTime
    {
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
        public DateTime CreationTime { get; set; }
    }
}
