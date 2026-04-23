using ADTO.DCloud.ApplicationForm;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.News.Dto
{

    [AutoMap(typeof(NewsEntity))]
    public class NewsDto:FullAuditedEntityDto<Guid>
    {
        /// <summary>
        /// 类型（枚举（NewsType）--1-新闻2-公告）
        /// </summary>
        /// <returns></returns>
        public string TypeId { get; set; }
        /// <summary>
        /// 所属类别主键
        /// </summary>
        /// <returns></returns>
        public string CategoryId { get; set; }
        /// <summary>
        /// 完整标题
        /// </summary>
        /// <returns></returns>
        [Required]
        [StringLength(200)]
        public string FullHead { get; set; }
        /// <summary>
        /// 标题颜色
        /// </summary>
        /// <returns></returns>
        [StringLength(128)]
        public string FullHeadColor { get; set; }
        /// <summary>
        /// 简略标题
        /// </summary>
        /// <returns></returns>
        [StringLength(128)]
        public string BriefHead { get; set; }
        /// <summary>
        /// 作者
        /// </summary>
        /// <returns></returns>
        [StringLength(128)]
        public string AuthorName { get; set; }
        /// <summary>
        /// 编辑
        /// </summary>
        /// <returns></returns>
        [StringLength(128)]
        public string CompileName { get; set; }
        /// <summary>
        /// Tag词
        /// </summary>
        /// <returns></returns>
        [StringLength(225)]
        public string TagWord { get; set; }
        /// <summary>
        /// 关键字
        /// </summary>
        /// <returns></returns>
        [StringLength(225)]
        public string Keyword { get; set; }
        /// <summary>
        /// 来源
        /// </summary>
        /// <returns></returns>
        [StringLength(200)]
        public string SourceName { get; set; }
        /// <summary>
        /// 来源地址
        /// </summary>
        /// <returns></returns>
        [StringLength(128)]
        public string SourceAddress { get; set; }
        /// <summary>
        /// 新闻内容
        /// </summary>
        /// <returns></returns>
        [StringLength(int.MaxValue)]
        public string NewsContent { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime? ReleaseTime { get; set; }
        /// <summary>
        /// 浏览量
        /// </summary>
        /// <returns></returns>
        public int PV { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateUserName { get; set; }
    }
}
