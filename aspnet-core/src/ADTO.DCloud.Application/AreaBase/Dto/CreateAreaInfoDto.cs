using System;
using System.ComponentModel;
using ADTOSharp.Application.Services.Dto;

namespace ADTO.DCloud.AreaBase.Dto
{
    /// <summary>
    /// 新增区域、国家、省市区 等信息
    /// </summary>
    public class CreateAreaInfoDto : EntityDto<Guid?>
    {
        /// <summary>
        /// 上级（区域、国家、省、市 四种）
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 级别（1=大区；2=国家；3=省份；4=市；5=区）冗余
        /// </summary>
        public int Level { get; set; }

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
        /// 是否有效、启用
        /// </summary>
        [Description("是否有效、启用")]
        public bool IsActive { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

    }
}
