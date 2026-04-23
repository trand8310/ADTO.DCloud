using ADTO.DCloud.Dto;
using ADTOSharp.Runtime.Validation;
using System;

namespace ADTO.DCloud.Customers.CustomerProductManage.Dto
{
    /// <summary>
    /// 客户产品分页列表
    /// </summary>
    public class PagedProductResultRequestDto : PagedAndSortedInputDto, IShouldNormalize
    {
        /// <summary>
        /// 客户Id
        /// </summary>
        public Guid CustomerId { get; set; }
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
