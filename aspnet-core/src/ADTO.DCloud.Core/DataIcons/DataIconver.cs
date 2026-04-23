using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using ADTOSharp.Domain.Entities;

namespace ADTO.DCloud.DataIcons
{
    /// <summary>
    /// 图标版本号表
    /// </summary>
    [Description("图标版本号表"), Table("DataIconver")]
    public class DataIconver : Entity<Guid>
    {
        /// <summary>
        /// 标识
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public string Ver { get; set; }
    }
}
