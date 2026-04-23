using ADTO.DCloud.Dto;
using ADTOSharp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Delegates.Dto
{
    /// <summary>
    /// 委托列表请求参数
    /// </summary>
    public class GetPageListInput : PagedAndSortedInputDto, IShouldNormalize
    {
        /// <summary>
        /// 委托人
        /// </summary>
        public string ToUserName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary> 
        /// 委托类型 1 待发委托 0或其他 审批委托
        /// </summary> 
        public int? Type { get; set; }

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
