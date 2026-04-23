using ADTOSharp.Application.Services.Dto;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Dto
{
    /// <summary>
    /// 数据权限返回
    /// </summary>
    public class PagedResultWithAuthDto<T> : PagedResultDto<T>
    {
        /// <summary>
        /// 权限字段
        /// </summary>
        public string AuthFields { get; set; } = "*";

        public PagedResultWithAuthDto(int totalCount, IReadOnlyList<T> items, string authFields = "*")
            : base(totalCount, items)
        {
            this.AuthFields = authFields;
        }
    }
}
