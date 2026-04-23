using ADTO.DCloud.Authorization.Users.Dto;
using ADTO.DCloud.Dto;
using ADTOSharp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Modules.Dto
{
    public class GetModulesInput : PagedAndSortedInputDto, IShouldNormalize
    {
        /// <summary>
        /// 父级ID
        /// </summary>
        public Guid? ParentId { get; set; }
        public string Filter { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "DisplayOrder,DisplayName";
            }

            Filter = Filter?.Trim();
        }
    }
}
