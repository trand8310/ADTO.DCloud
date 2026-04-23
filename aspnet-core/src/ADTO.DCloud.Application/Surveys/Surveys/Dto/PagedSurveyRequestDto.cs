using ADTOSharp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.Surveys.Surveys.Dto
{
    /// <summary>
    /// 分页查询Dto
    /// </summary>
    public class PagedSurveyRequestDto : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// 关键词
        /// </summary>
        public string keyword { get; set; }

    }
}
