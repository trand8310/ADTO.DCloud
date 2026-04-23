using ADTO.DCloud.Dto;
using ADTOSharp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Schemes.Dto
{
    /// <summary>
    /// 历史流程模板记录
    /// </summary>
    public class GetHistorySchemePageInput : PagedAndSortedInputDto, IShouldNormalize
    {
        public string Filter { get; set; }
        /// <summary>
        /// 流程模板信息主键
        /// </summary>
        public Guid SchemeInfoId { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "CreationTime";
            }
            Filter = Filter?.Trim();
        }
    }
}
