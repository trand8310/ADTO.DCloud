using ADTOSharp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADTO.DCloud.DataArea.Dto
{
    /// <summary>
    /// 区域
    /// </summary>
    [AutoMap(typeof(DataArea))]
    public class DataAreaDto
    {
        /// <summary>
        /// 父级主键
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 区域编码
        /// </summary>
        public string AreaCode { get; set; }

        /// <summary>
        /// 区域名称
        /// </summary>
        public string AreaName { get; set; }

        /// <summary>
        /// 快速查询
        /// </summary>
        public string QuickQuery { get; set; }

        /// <summary>
        /// 简拼
        /// </summary>
        public string SimpleSpelling { get; set; }

        /// <summary>
        /// 层次
        /// </summary>
        public int? Layer { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int DisplayOrder { get; set; }

    }
}
