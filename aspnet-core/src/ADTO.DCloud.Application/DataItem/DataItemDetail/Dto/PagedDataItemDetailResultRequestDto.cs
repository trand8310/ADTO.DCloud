using ADTOSharp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.DataItem.Dto
{
    public class PagedDataItemDetailResultRequestDto: PagedResultRequestDto
    {
        /// <summary>
        /// 关键词
        /// </summary>
        public string KeyWord  { get; set; }

        /// <summary>
        /// 分类主键
        /// </summary>
        public Guid? ItemId { get; set; }
    }
}
