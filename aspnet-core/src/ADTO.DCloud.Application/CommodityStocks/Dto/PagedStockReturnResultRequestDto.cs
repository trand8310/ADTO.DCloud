using ADTO.DCloud.Infrastructure;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.Extensions;
using ADTOSharp.Runtime.Validation;
using ADTOSharp.Timing;
using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ADTO.DCloud.CommodityStocks.Dto
{
    public class PagedStockReturnResultRequestDto : PagedAndSortedResultRequestDto, IShouldNormalize
    {
        /// <summary>
        /// 根据工号查询
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 申请人
        /// </summary>
        public string Keyword { get; set; }

        /// <summary>
        /// 申请类型
        /// </summary>
        public int? ApplyType { get; set; }

        /// <summary>
        /// 归还状态
        /// -1=所有；0=未归还；1=已归还； 100=离职未归还  为空也为所有
        /// </summary>
        public int? ReturnStatus { get; set; }

        /// <summary>
        /// 产品分类
        /// </summary>
        public Guid CategoryId { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        [DisableDateTimeNormalization]
        [JsonConverter(typeof(OnlyDateConverter))]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        [DisableDateTimeNormalization]
        [JsonConverter(typeof(OnlyDateConverter))]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 公司Id
        /// </summary>
        public Guid? CompanyId { get; set; }

        /// <summary>
        /// 部门Id
        /// </summary>
        public Guid? DeptId { get; set; }

        /// <summary>
        /// 产品Id
        /// </summary>
        public Guid? ProductId { get; set; }
        public void Normalize()
        {
            if (Sorting.IsNullOrWhiteSpace())
            {
                Sorting = " ReturnStatus ASC,BackTime DESC ";
            }
            Sorting = DtoSortingHelper.ReplaceSorting(Sorting, s =>
            {
                return s;
            });
        }
    }
}
