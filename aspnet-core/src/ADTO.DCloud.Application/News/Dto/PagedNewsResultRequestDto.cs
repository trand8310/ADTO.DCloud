using ADTO.DCloud.Dto;
using ADTOSharp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.News.Dto
{
    public class PagedNewsResultRequestDto : PagedAndSortedInputDto, IShouldNormalize
    {
        /// <summary>
        /// 过来条件
        /// </summary>
        public string Filter { get; set; }


        /// <summary>
        /// 新闻类型
        /// </summary>
        public int? TypeId { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = " CreationTime asc";
            }
            Filter = Filter?.Trim();
        }
    }
}
