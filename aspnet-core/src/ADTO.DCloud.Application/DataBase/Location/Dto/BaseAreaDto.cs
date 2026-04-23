using System;
using ADTOSharp.AutoMapper;
using ADTOSharp.Application.Services.Dto;
using ADTO.DCloud.AreaBase;

namespace ADTO.DCloud.DataBase.Location.Dto
{
    /// <summary>
    /// 地区（亚洲类）
    /// </summary>
    [AutoMap(typeof(Base_Area))]
    public class BaseAreaDto:EntityDto<int>
    {
        /// <summary>
        /// 父级
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 英文名称(全称)
        /// </summary>
        public string EnglishName { get; set; }

        /// <summary>
        /// 代码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 图片
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// 添加人
        /// </summary>
        public string CreatePerson { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        public string UpdatePerson { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public int IsEnable { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
