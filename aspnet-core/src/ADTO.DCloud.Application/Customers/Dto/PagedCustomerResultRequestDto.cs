using System;
using ADTO.DCloud.Dto;
using ADTOSharp.Runtime.Validation;

namespace ADTO.DCloud.Customers.Dto
{
    public class PagedCustomerResultRequestDto : PagedAndSortedInputDto, IShouldNormalize
    {
        /// <summary>
        /// 关键词
        /// </summary>
        public string KeyWord { get; set; }

        /// <summary>
        /// 部门Id
        /// </summary>
        public Guid? DepartmentId { get; set; }

        /// <summary>
        /// 客户状态 字典
        /// </summary>
        public string CustomerState { get; set; }

        /// <summary>
        /// 客户评级 字典
        /// </summary>
        public string CustomerLevel { get; set; }

        /// <summary>
        /// 创建开始日期
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 创建结束日期
        /// </summary>
        public DateTime? EndDate { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "CreationTime DESC";
            }
        }
    }
}
