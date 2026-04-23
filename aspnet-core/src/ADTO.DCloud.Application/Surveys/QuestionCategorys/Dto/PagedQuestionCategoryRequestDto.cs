
using ADTOSharp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Surveys.QuestionCategorys.Dto
{
    /// <summary>
    /// 分页
    /// </summary>
    public class PagedQuestionCategoryRequestDto: PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// 关键词
        /// </summary>
        public string keyWord { get; set; }
    }
}
