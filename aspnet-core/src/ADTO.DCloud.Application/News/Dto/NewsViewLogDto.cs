using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.News.Dto
{
    [AutoMap(typeof(NewsViewLog))]
    public class NewsViewLogDto : CreationAuditedEntityDto<Guid>
    {
        /// <summary>
        /// 新闻Id
        /// </summary>
        public Guid NewsId { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 浏览人
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string DepartmentName { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get; set; }
    }
}
