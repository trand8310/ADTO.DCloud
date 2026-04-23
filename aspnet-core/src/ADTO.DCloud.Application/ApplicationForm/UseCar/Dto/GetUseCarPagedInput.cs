using ADTO.DCloud.Dto;
using ADTO.DCloud.Infrastructure;
using ADTOSharp.Timing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ADTO.DCloud.ApplicationForm.UseCar.Dto
{
    public class GetUseCarPagedInput : PagedAndSortedInputDto
    {

        /// <summary>
        /// 
        /// </summary>
        public string Keyword { get; set; }

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
        public Guid? DepartmentId { get; set; }


        /// <summary>
        /// 车牌号
        /// </summary>
        public string CarNum { get; set; }

        /// <summary>
        /// 用车类型
        /// 数据字典
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 司机
        /// </summary>
        public string Driver { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = " CreationTime desc";
            }

            Keyword = Keyword?.Trim();
        }
    }
}
