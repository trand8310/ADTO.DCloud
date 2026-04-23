using ADTO.DCloud.Dto;
using ADTOSharp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.DeptRoles.Dto
{
    public class GetDeptRolePageInput : PagedAndSortedInputDto, IShouldNormalize
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public string KeyWord { get; set; }

        /// <summary>
        /// 分类
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool? IsActive { get; set; }
         
        public string Filter { get; set; }


        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = " CreationTime desc,DisplayOrder";
            }

            Filter = Filter?.Trim();
        }

    }
}
