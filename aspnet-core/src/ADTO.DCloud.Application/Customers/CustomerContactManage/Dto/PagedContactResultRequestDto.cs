using ADTO.DCloud.Dto;
using ADTOSharp.Runtime.Validation;
using System;


namespace ADTO.DCloud.Customers.CustomerContactManage.Dto
{
    public class PagedContactResultRequestDto : PagedAndSortedInputDto, IShouldNormalize
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
