using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using ADTOSharp.Domain.Entities.Auditing;
using System;

namespace ADTO.DCloud.DataItem.Dto
{
    [AutoMap(typeof(DataItem))]
    public class DataItemDto : EntityDto<Guid>, IHasCreationTime
    {
        public DataItemDto() { }

        /// <summary>
        /// 父级主键
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 分类编码
        /// </summary>
        public string ItemCode { get; set; }

        /// <summary>
        /// 分类名称
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        public DateTime CreationTime { get; set; }
    }
}
