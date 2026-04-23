using ADTO.DCloud.Dto;
using ADTOSharp.Runtime.Validation;
using System;

namespace ADTO.DCloud.Customers.CustomerLogManage.Dto
{
    public class PagedCustomerLogResultRequestDto : PagedAndSortedInputDto, IShouldNormalize
    {
        /// <summary>
        /// 关键词
        /// </summary>
        public string KeyWord { get; set; }

        /// <summary>
        /// 客户Id
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// 创建开始日期
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 创建结束日期
        /// </summary>
        public DateTime? EndDate { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                if (string.IsNullOrEmpty(Sorting))
                {
                    Sorting = "CreationTime ";
                }
            }
        }
    }
}
