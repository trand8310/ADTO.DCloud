using ADTO.DCloud.Dto;
using ADTOSharp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Customers.CustomerShareManage.Dto
{
    public class PagedShareRecordResultRequestDto : PagedAndSortedInputDto, IShouldNormalize
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
                    Sorting = "FollowTime DESC";
                }
            }
        }
    }
}
