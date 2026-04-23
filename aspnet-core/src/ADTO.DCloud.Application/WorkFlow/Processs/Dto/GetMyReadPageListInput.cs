using ADTO.DCloud.Dto;
using ADTOSharp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.WorkFlow.Processs.Dto
{
    /// <summary>
    /// 传阅
    /// </summary>
    public class GetMyReadPageListInput : PagedAndSortedInputDto, IShouldNormalize
    {

        /// <summary>
        /// 查询关键字
        /// </summary>
        public string Keyword { get; set; }
        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// 结束流程
        /// </summary>
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// 流程模板编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 流程模板名称
        /// </summary>
        public string SchemaName { get; set; }
        /// <summary>
        /// 表单关键字
        /// </summary>
        public string FormKeyword { get; set; }

        /// <summary>
        /// 发起人Id
        /// </summary>
        public Guid? UserId { get; set; }
        /// <summary>
        /// 查询条件
        /// </summary>
        public string Where { get; set; }

        public string Filter { get; set; }
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
