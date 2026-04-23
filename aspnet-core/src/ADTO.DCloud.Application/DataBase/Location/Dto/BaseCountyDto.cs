using ADTO.DCloud.AreaBase;
using ADTOSharp.Application.Services.Dto;
using ADTOSharp.AutoMapper;
using System;

namespace ADTO.DCloud.DataBase.Location.Dto
{
    /// <summary>
    /// 县/区表
    /// </summary>
    [AutoMap(typeof(Base_County))]
    public class BaseCountyDto : EntityDto<int>
    {
        /// <summary>
        /// 区域Id
        /// </summary>
        public int AreaId { get; set; }

        /// <summary>
        /// 国家Id
        /// </summary>
        public int CountryId { get; set; }

        /// <summary>
        /// 省份Id
        /// </summary>
        public int ProvinceId { get; set; }

        /// <summary>
        /// 城市Id
        /// </summary>
        public int CityId { get; set; }

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
