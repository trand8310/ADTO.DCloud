using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;

namespace ADTO.DCloud.DataItem.Dto
{
    [AutoMap(typeof(DataItem))]
    public class DataItemTreeDto : EntityDto<Guid>
    {
        /// <summary>
        /// 父级主键
        /// </summary>
        /// <returns></returns>
        public string ParentId { get; set; }
        /// <summary>
        /// 父级名称
        /// </summary>
        public string ParentName { get; set; }
        /// <summary>
        /// 分类编码
        /// </summary>
        /// <returns></returns>
        public string ItemCode { get; set; }
        /// <summary>
        /// 分类名称
        /// </summary>
        /// <returns></returns>

        public string ItemName { get; set; }

        /// <summary>
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

        /// <summary>
        /// 子节点
        /// </summary>
        public List<DataItemTreeDto> children { get; set; }

        /// <summary>
        /// 父级
        /// </summary>
        public DataItemTreeDto? Parent { get; set; }
    }
}
