using ADTO.DCloud.Modules.Dto;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.DataItem.Dto
{

    [AutoMapFrom(typeof(DataItemDetailDto))]
    public class DataItemDetailTreeDto : EntityDto<Guid>
    {

        public DataItemDetailTreeDto() { Children = new List<DataItemDetailTreeDto>(); }
        public List<DataItemDetailTreeDto> Children { get; set; }
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
