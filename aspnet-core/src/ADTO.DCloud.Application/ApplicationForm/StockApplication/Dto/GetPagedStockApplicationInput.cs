using ADTO.DCloud.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.ApplicationForm.Stocks.Dto
{
    public class GetPagedStockApplicationInput : PagedAndSortedInputDto
    {
        /// <summary>
        /// 分类
        /// </summary>
        public string Category { get; set; }

        public string Filter { get; set; }


        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = " CreationTime desc";
            }

            Filter = Filter?.Trim();
        }
    }
}
