using ADTO.DCloud.Dto;
using ADTOSharp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.News.Dto
{

    public class GetNewsPagedInput : PagedAndSortedInputDto, IShouldNormalize
    {
        public string Filter { get; set; }


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
