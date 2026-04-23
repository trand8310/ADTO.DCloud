using ADTO.DCloud.Dto;
using ADTOSharp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.DataAuthorizes.Dto
{
    /// <summary>
    /// 数据权限分页参数
    /// </summary>
    public class GeDataAuthorizePagedInput : PagedAndSortedInputDto, IShouldNormalize
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public string KeyWord { get; set; }
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
